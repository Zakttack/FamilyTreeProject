using Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.Configuration.Models;
using VirtualFamilyMuseumLibrary.Configuration.Repositories;

namespace VirtualFamilyMuseumLibraryTest.Configuration.Repositories
{
    [TestFixture]
    [Category("Integration")]
    public class FamilyVaultTest
    {
        private IHost? host;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // Verify the bootstrap environment variable exists
            string? configUri = Environment.GetEnvironmentVariable("FAMILY_CONFIGURATION_URI");
            if (string.IsNullOrWhiteSpace(configUri))
            {
                Assert.Fail(
                    "Integration tests require FAMILY_CONFIGURATION_URI environment variable. " +
                    "Set it to your Azure App Configuration endpoint: " +
                    "https://<your-config>.azconfig.io"
                );
            }
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.AddFamilyConfiguration(allowLocalJsonFallback: false);
            builder.AddFamilyVault();
            host = builder.Build();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            host?.Dispose();
        }

        [Test]
        [Order(1)]
        public void ShouldLoadFamilyVaultConfigFromAppConfiguration()
        {
            // Arrange & Act
            FamilyVaultConfig config = host!.Services.GetRequiredService<IOptions<FamilyVaultConfig>>().Value;
            Assert.Multiple(() =>
            {
                Assert.That(config.Uri, Is.Not.Null.And.Not.Empty, "FamilyVault Uri should be populated");
                Assert.That(Uri.IsWellFormedUriString(config.Uri, UriKind.Absolute), Is.True,
                    "FamilyVault Uri should be a valid absolute URI");
            });
            TestContext.Out.WriteLine($"✓ FamilyVault Uri loaded: {config.Uri}");
        }

        [Test]
        [Order(2)]
        public void ShouldRegisterSensitiveConstantRepository()
        {
            // Arrange & Act
            ISensitiveConstantRepository? repository = host!.Services.GetService<ISensitiveConstantRepository>();

            // Assert
            Assert.That(repository, Is.Not.Null, "ISensitiveConstantRepository should be registered");
            Assert.That(repository, Is.InstanceOf<FamilyVault>(), 
                "Repository should be FamilyVault implementation");
            
            TestContext.Out.WriteLine($"✓ ISensitiveConstantRepository registered: {repository.GetType().Name}");
        }

        [Test]
        [Order(3)]
        public async Task ShouldReadSecretFromKeyVault()
        {
            // Arrange
            ISensitiveConstantRepository repository = host!.Services.GetRequiredService<ISensitiveConstantRepository>();
            const string secretName = "FamilyInsights--ConnectionString";

            // Act
            try
            {
                string secretValue = await repository.GetSecretAsync(secretName);

                // Assert
                Assert.That(secretValue, Is.Not.Null.And.Not.Empty, 
                    $"Secret '{secretName}' should have a non-empty value");
                
                TestContext.Out.WriteLine($"✓ Successfully read secret '{secretName}' from Key Vault");
                TestContext.Out.WriteLine($"  Value length: {secretValue.Length} characters");
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                Assert.Fail($"Secret '{secretName}' not found in Key Vault. " +
                        "Please update this test with an actual secret name from your vault, " +
                        "or create this secret using: az keyvault secret set --vault-name <vault> --name TestSecret --value TestValue123");
            }
            catch (RequestFailedException ex) when (ex.Status == 403)
            {
                Assert.Fail($"Access denied to secret '{secretName}'. " +
                        "Ensure your identity has 'Key Vault Secrets User' role assigned. " +
                        "RBAC changes can take 5-10 minutes to propagate.");
            }
        }

        [Test]
        [Order(4)]
        public async Task ShouldHandleNonExistentSecretGracefully()
        {
            // Arrange
            ISensitiveConstantRepository repository = host!.Services.GetRequiredService<ISensitiveConstantRepository>();
            string nonExistentSecret = "ThisSecretDefinitelyDoesNotExist-12345";

            // Act & Assert
            var ex = Assert.ThrowsAsync<RequestFailedException>(
                async () => await repository.GetSecretAsync(nonExistentSecret));
            
            Assert.That(ex.Status, Is.EqualTo(404), "Should return 404 for non-existent secret");
            
            TestContext.Out.WriteLine($"✓ Correctly threw RequestFailedException for non-existent secret");
        }

        [Test]
        [Order(5)]
        public void ShouldRejectInvalidSecretNames()
        {
            // Arrange
            ISensitiveConstantRepository repository = host!.Services.GetRequiredService<ISensitiveConstantRepository>();
            
            // Azure Key Vault has restrictions on secret names:
            // - Only alphanumeric characters and hyphens
            // - Cannot start or end with a hyphen
            // - Maximum 127 characters
            // Test with an invalid name (contains underscore and special chars)
            const string invalidSecretName = "Invalid_Secret_Name_With_Underscores_And_@_Special_Chars!";

            // Act & Assert
            var ex = Assert.ThrowsAsync<RequestFailedException>(
                async () => await repository.GetSecretAsync(invalidSecretName));
            
            // Azure Key Vault returns 400 (Bad Request) for invalid secret names
            Assert.That(ex!.Status, Is.EqualTo(400), 
                $"Should return 400 for invalid secret name format. Got {ex.Status}: {ex.Message}");
            
            TestContext.Out.WriteLine($"✓ Correctly threw RequestFailedException (400) for invalid secret name");
            TestContext.Out.WriteLine($"  Error message: {ex.Message}");
        }
    }
}
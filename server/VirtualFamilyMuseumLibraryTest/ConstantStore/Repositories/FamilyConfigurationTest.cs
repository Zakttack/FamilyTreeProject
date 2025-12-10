using Microsoft.Extensions.Hosting;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.ConstantStore.Models;
using VirtualFamilyMuseumLibrary.ConstantStore.Repositories;

namespace VirtualFamilyMuseumLibraryTest.ConstantStore.Repositories
{
    [TestFixture]
    [Category("Integration")]
    public class FamilyConfigurationTest
    {
        private IHost? host;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            Environment.SetEnvironmentVariable("FAMILY_CONFIGURATION_URI", "https://appconfig-virtual-family-museum.azconfig.io");
            HostApplicationBuilder? builder = Host.CreateApplicationBuilder();
            builder.AddFamilyConfiguration(allowLocalJsonFallback: false);
            host = builder.Build();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            host?.Dispose();
        }

        [Test]
        [Order(1)]
        public void ShouldRegisterNonSensitiveConstantRepository()
        {
            // Arrange & Act
            INonSensitiveConstantRepository? repository = FamilyUtils.NonSensitiveConstantRepository;

            // Assert
            Assert.That(repository, Is.Not.Null, "INonSensitiveConstantRepository should be registered");
            Assert.That(repository, Is.InstanceOf<FamilyConfiguration>(), 
                "Repository should be FamilyConfiguration implementation");
            
            TestContext.Out.WriteLine($"INonSensitiveConstantRepository registered: {repository.GetType().Name}");
        }

        [Test]
        [Order(2)]
        public void ShouldReadNonSensitiveValueFromAppConfiguration()
        {
            // Arrange
            INonSensitiveConstantRepository? repository = FamilyUtils.NonSensitiveConstantRepository;
            Assert.That(repository, Is.Not.Null);
            // Act
            string? vaultUri = repository.GetValue("FamilyVault:Uri");

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(vaultUri, Is.Not.Null.And.Not.Empty,
                    "Should be able to read FamilyVault:Uri from App Configuration");
                Assert.That(Uri.IsWellFormedUriString(vaultUri, UriKind.Absolute), Is.True,
                    "FamilyVault:Uri should be a valid absolute URI");
            });

            TestContext.Out.WriteLine($"✓ Read from App Configuration: FamilyVault:Uri = {vaultUri}");
        }

        [Test]
        [Order(3)]
        public void ShouldBindConfigurationSection()
        {
            // Arrange
            INonSensitiveConstantRepository? repository = FamilyUtils.NonSensitiveConstantRepository;
            Assert.That(repository, Is.Not.Null);
            // Act
            FamilyVaultConfig? vaultConfig = repository.BindSection<FamilyVaultConfig>("FamilyVault");

            // Assert
            Assert.That(vaultConfig, Is.Not.Null, "Should bind FamilyVault section");
            Assert.That(vaultConfig.Uri, Is.Not.Null.And.Not.Empty, "Bound config should have Uri");
            
            TestContext.Out.WriteLine($"✓ Successfully bound FamilyVault section");
            TestContext.Out.WriteLine($"  {vaultConfig}");
        }
    }
}
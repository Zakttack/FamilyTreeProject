using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualFamilyMuseumLibrary;

namespace VirtualFamilyMuseumLibraryTest
{
    public class ExtensionsTest
    {
        internal const string FAMILY_CONFIGURATION_TEST_ENDPOINT = "https://app-config-virtual-family-museum-test.azconfig.io";
        internal const string FAMILY_VAULT_TEST_ENDPOINT = "https://kv-vfm-test.vault.azure.net/";
        private IHost host;
        private IConfiguration constantStore;
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("FamilyConfigurationTest__Endpoint", FAMILY_CONFIGURATION_TEST_ENDPOINT);
            Environment.SetEnvironmentVariable("FamilyVaultTest__Endpoint", FAMILY_VAULT_TEST_ENDPOINT);
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddConstantStorePipeline(ExecutionTypes.Test);
            constantStore = builder.Configuration;
            builder.AddFamilyInsights(ExecutionTypes.Test);
            host = builder.Build();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            host.Dispose();
        }

        [Test]
        public void ShouldReadConfigurationEndpoint()
        {
            Assert.That(constantStore["FamilyConfigurationTest:Endpoint"], Is.EqualTo(FAMILY_CONFIGURATION_TEST_ENDPOINT));
        }

        [Test]
        public void ShouldReadVaultEndpoint()
        {
            Assert.That(constantStore["FamilyVaultTest:Endpoint"], Is.EqualTo(FAMILY_VAULT_TEST_ENDPOINT));
        }

        [Test]
        public void ShouldReadConfigurationValue()
        {
            Assert.That(constantStore["TestKey"], Is.EqualTo("TestValue"));
        }

        [Test]
        public void ShouldReadVaultSecretValue()
        {
            Assert.That(constantStore["TestSecret"], Is.EqualTo("HelloWorld"));
        }

        [Test]
        public void ShouldRegisterApplicationInsightsLoggerProvider()
        {
            IEnumerable<ILoggerProvider>? providers = host.Services.GetServices<ILoggerProvider>();
            Assert.That(providers.Any(p => p.GetType().Name.Contains("ApplicationInsights")), Is.True);
        }

        [Test]
        public void ShouldLogWithoutThrowing()
        {
            Assert.DoesNotThrow(() =>
            {
                ILogger<ExtensionsTest> logger = host.Services.GetRequiredService<ILogger<ExtensionsTest>>();
                logger.LogDebug("This is a debug message.");
                logger.LogInformation("This is an information message");
                logger.LogWarning("This is a warning message.");
                logger.LogError("This is an error message.");
                logger.LogCritical("This is a critical message.");
            });
        }
    }
}
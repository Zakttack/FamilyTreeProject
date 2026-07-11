using Microsoft.Extensions.Configuration;
using VirtualFamilyMuseumLibrary;

namespace VirtualFamilyMuseumLibraryTest
{
    public class ExtensionsTest
    {
        internal const string FAMILY_CONFIGURATION_TEST_ENDPOINT = "https://app-config-virtual-family-museum-test.azconfig.io";
        internal const string FAMILY_VAULT_TEST_ENDPOINT = "https://kv-vfm-test.vault.azure.net/";
        private IConfiguration constantStore;
        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("FamilyConfigurationTest__Endpoint", FAMILY_CONFIGURATION_TEST_ENDPOINT);
            Environment.SetEnvironmentVariable("FamilyVaultTest__Endpoint", FAMILY_VAULT_TEST_ENDPOINT);
            IConfigurationBuilder builder = new ConfigurationBuilder().AddConstantStorePipeline(ExecutionTypes.Test);
            constantStore = builder.Build();
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
    }
}
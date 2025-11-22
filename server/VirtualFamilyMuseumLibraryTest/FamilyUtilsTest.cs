using Microsoft.Extensions.Hosting;
using VirtualFamilyMuseumLibrary;

namespace VirtualFamilyMuseumLibraryTest
{
    public class FamilyUtilsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FamilyConfigurationResourceShouldConnect()
        {
            Environment.SetEnvironmentVariable("FAMILY_CONFIGURATION_URI", "https://appconfig-virtual-family-museum.azconfig.io");
            var builder = Host.CreateApplicationBuilder();
            builder.AddFamilyConfiguration(allowLocalJsonFallback: false);
            Assert.That(Environment.GetEnvironmentVariable("FAMILY_CONFIGURATION_URI"), Is.EqualTo("https://appconfig-virtual-family-museum.azconfig.io"));
        }

        [Test]
        public void FamilyConfigurationResourceShouldNotConnect()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var builder = Host.CreateApplicationBuilder();
                builder.AddFamilyConfiguration(allowLocalJsonFallback: false);
            });
        }
    }
}
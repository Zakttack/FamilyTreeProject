using Microsoft.Extensions.Hosting;
using VirtualFamilyMuseumLibrary;

namespace VirtualFamilyMuseumLibraryTest
{
    [TestFixture]
    [Category("Unit")]
    public class FamilyUtilsTest
    {

        [Test]
        public void FamilyConfigurationResourceShouldConnect()
        {
            Environment.SetEnvironmentVariable("FAMILY_CONFIGURATION_URI", "https://appconfig-virtual-family-museum.azconfig.io");
            var builder = Host.CreateApplicationBuilder();
            builder.AddFamilyConfiguration(allowLocalJsonFallback: false);
            Assert.That(Environment.GetEnvironmentVariable("FAMILY_CONFIGURATION_URI"), Is.EqualTo("https://appconfig-virtual-family-museum.azconfig.io"));
        }
    }
}
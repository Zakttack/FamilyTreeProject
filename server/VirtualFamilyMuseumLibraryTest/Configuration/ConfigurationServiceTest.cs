using VirtualFamilyMuseumLibrary.Configuration;
using VirtualFamilyMuseumLibrary.Configuration.Models;
using VirtualFamilyMuseumLibraryTest.Configuration.Repositories;

namespace VirtualFamilyMuseumLibraryTest.Configuration
{
    [TestFixture]
    [Category("Unit")]
    public class ConfigurationServiceTest
    {
        private ConfigurationService service;
        [SetUp]
        public void SetUp()
        {
            service = new(new NonSensitiveConstantRepositoryTestDouble(), new SensitiveConstantRepositoryTestDouble());
        }

        [Test]
        public async Task ShouldReturnFamilyInsightsConfig()
        {
            FamilyInsightsConfig expectedConfig = new()
            {
                ConnectionString = "This is the connection string to family insights"
            };
            FamilyInsightsConfig actualConfig = await service.GetFamilyInsightsConfig();
            Assert.That(actualConfig, Is.EqualTo(expectedConfig));
        }
    }
}
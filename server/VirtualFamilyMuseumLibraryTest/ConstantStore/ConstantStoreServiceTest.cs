using VirtualFamilyMuseumLibrary.ConstantStore;
using VirtualFamilyMuseumLibrary.ConstantStore.Models;
using VirtualFamilyMuseumLibraryTest.ConstantStore.Repositories;

namespace VirtualFamilyMuseumLibraryTest.ConstantStore
{
    [TestFixture]
    [Category("Unit")]
    public class ConstantStoreServiceTest
    {
        private ConstantStoreService service;
        [SetUp]
        public void SetUp()
        {
            service = new(new NonSensitiveConstantRepositoryTestDouble(), new SensitiveConstantRepositoryTestDouble());
        }

        [Test]
        public void ShouldReturnFamilyInsightsConfig()
        {
            FamilyInsightsConfig expectedConfig = new()
            {
                ConnectionString = "This is the connection string to family insights"
            };
            FamilyInsightsConfig actualConfig = service.GetFamilyInsightsConfig();
            Assert.That(actualConfig, Is.EqualTo(expectedConfig));
        }
    }
}
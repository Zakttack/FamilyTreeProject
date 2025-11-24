using Microsoft.Extensions.Configuration;
using VirtualFamilyMuseumLibrary.Configuration.Models;
using VirtualFamilyMuseumLibrary.Configuration.Repositories;

namespace VirtualFamilyMuseumLibraryTest.Configuration.Repositories
{
    public class FamilyConfigurationTest
    {
        private IConstantRepository repository;
        [SetUp]
        public void Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string,string?>
                {
                    ["FamilyVault:Uri"] = "https://vault.example.net"
                }).Build();
            repository = new FamilyConstantRepository(configuration);
        }

        [Test]
        public void ShouldReadKeyVaultUri()
        {
            string expected = "https://vault.example.net";
            Assert.That(repository.GetValue("FamilyVault:Uri") , Is.EqualTo(expected));
        }

        [Test]
        public void ShouldNotReadOtherAttribute()
        {
            Assert.That(repository.GetValue("SomeAttribute"), Is.Null);
        }

        [Test]
        public void ShouldModelKeyVault()
        {
            FamilyVaultConfig expected = new()
            {
                Uri = "https://vault.example.net"
            };
            FamilyVaultConfig? actual = repository.BindSection<FamilyVaultConfig>("FamilyVault");
            Assert.That(actual, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(actual.Uri, Is.EqualTo("https://vault.example.net"));
                Assert.That(actual, Is.EqualTo(expected));
            });
        }

        [Test]
        public void ShouldNotModelKeyVault()
        {
            Assert.That(repository.BindSection<FamilyVaultConfig>("FamilyVault:Uri"), Is.Null);
        }

        [Test]
        public void ShouldModelAsString()
        {
            string? actual = repository.BindSection<string>("FamilyVault:Uri");
            Assert.That(actual, Is.EqualTo(repository.GetValue("FamilyVault:Uri")));
        }

        [Test]
        public void ShouldNotModelAsString()
        {
            Assert.Throws<InvalidOperationException>(() => 
            {
                repository.BindSection<string>("FamilyVault");
            });
        }
    }
}
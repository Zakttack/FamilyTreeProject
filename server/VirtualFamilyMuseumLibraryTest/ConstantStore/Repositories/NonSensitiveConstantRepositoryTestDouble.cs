using VirtualFamilyMuseumLibrary.ConstantStore.Models;
using VirtualFamilyMuseumLibrary.ConstantStore.Repositories;

namespace VirtualFamilyMuseumLibraryTest.ConstantStore.Repositories
{
    public class NonSensitiveConstantRepositoryTestDouble : INonSensitiveConstantRepository
    {
        private readonly IReadOnlyDictionary<string,object> constantSections = new Dictionary<string,object>()
        {
            {"FamilyVault", new FamilyVaultConfig(){Uri = "https://myvaulturi.com"}}
        };
        private readonly IReadOnlyDictionary<string,string> constantStore = new Dictionary<string,string>()
        {
            {"FamilyVault:Uri", "https://myvaulturi.com"}
        };

        public T? BindSection<T>(string section)
        {
            return constantSections.TryGetValue(section, out var configSection) && configSection is T t ? t : default;
        }

        public string? GetValue(string key)
        {
            return constantStore.TryGetValue(key, out string? value) ? value : null;
        }
    }
}
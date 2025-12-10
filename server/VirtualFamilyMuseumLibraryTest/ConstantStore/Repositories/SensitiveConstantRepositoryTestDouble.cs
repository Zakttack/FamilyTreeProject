using VirtualFamilyMuseumLibrary.ConstantStore.Repositories;

namespace VirtualFamilyMuseumLibraryTest.ConstantStore.Repositories
{
    public class SensitiveConstantRepositoryTestDouble : ISensitiveConstantRepository
    {
        private readonly IReadOnlyDictionary<string,string> secretStore = new Dictionary<string,string>()
        {
            {"FamilyInsights--ConnectionString", "This is the connection string to family insights"}
        };

        public string GetSecret(string name)
        {
            return secretStore[name];
        }
    }
}
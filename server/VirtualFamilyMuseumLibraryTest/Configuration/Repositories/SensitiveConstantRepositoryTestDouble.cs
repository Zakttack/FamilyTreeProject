using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualFamilyMuseumLibrary.Configuration.Repositories;

namespace VirtualFamilyMuseumLibraryTest.Configuration.Repositories
{
    public class SensitiveConstantRepositoryTestDouble : ISensitiveConstantRepository
    {
        private readonly IReadOnlyDictionary<string,string> secretStore = new Dictionary<string,string>()
        {
            {"FamilyInsights--ConnectionString", "This is the connection string to family insights"}
        };

        public Task<string> GetSecretAsync(string name)
        {
            return Task.FromResult(secretStore[name]);
        }
    }
}
using Microsoft.Extensions.Configuration;

namespace VirtualFamilyMuseumLibrary.Configuration.Repositories
{
    public class FamilyConstantRepository(IConfiguration config) : IConstantRepository
    {
        private readonly IConfiguration config = config;

        public C? BindSection<C>(string key)
        {
            IConfigurationSection section = config.GetSection(key);
            return section.Exists() ? section.Get<C>() : default;
        }

        public string? GetValue(string key)
        {
            return config[key];
        }
    }
}
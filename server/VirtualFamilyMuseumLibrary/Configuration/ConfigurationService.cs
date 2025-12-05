using VirtualFamilyMuseumLibrary.Configuration.Models;
using VirtualFamilyMuseumLibrary.Configuration.Repositories;

namespace VirtualFamilyMuseumLibrary.Configuration
{
    public class ConfigurationService(INonSensitiveConstantRepository nonSensitiveConstantRepository, ISensitiveConstantRepository sensitiveConstantRepository)
    {
        private readonly INonSensitiveConstantRepository nonSensitiveRepository = nonSensitiveConstantRepository;
        private readonly ISensitiveConstantRepository sensitiveRepository = sensitiveConstantRepository;

        public async Task<FamilyInsightsConfig> GetFamilyInsightsConfig()
        {
            string familyInsightsConnectionString = await sensitiveRepository.GetSecretAsync("FamilyInsights--ConnectionString");
            return new FamilyInsightsConfig()
            {
                ConnectionString = familyInsightsConnectionString
            };
        }
    }
}
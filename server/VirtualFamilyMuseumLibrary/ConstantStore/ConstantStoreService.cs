using VirtualFamilyMuseumLibrary.ConstantStore.Models;
using VirtualFamilyMuseumLibrary.ConstantStore.Repositories;

namespace VirtualFamilyMuseumLibrary.ConstantStore
{
    public class ConstantStoreService(INonSensitiveConstantRepository nonSensitiveConstantRepository, ISensitiveConstantRepository sensitiveConstantRepository)
    {
        private readonly INonSensitiveConstantRepository nonSensitiveRepository = nonSensitiveConstantRepository;
        private readonly ISensitiveConstantRepository sensitiveRepository = sensitiveConstantRepository;

        public FamilyInsightsConfig GetFamilyInsightsConfig()
        {
            string familyInsightsConnectionString = sensitiveRepository.GetSecret("FamilyInsights--ConnectionString");
            return new FamilyInsightsConfig()
            {
                ConnectionString = familyInsightsConnectionString
            };
        }
    }
}
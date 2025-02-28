using Azure.Data.AppConfiguration;
using Azure.Identity;

namespace FamilyTreeLibrary.Data
{
    public class FamilyTreeConfiguration
    {
        private const string API_CONFIGURATION_URI = "https://familytreeconfiguration.azconfig.io";
        private readonly ConfigurationClient client;

        public FamilyTreeConfiguration()
        {
            client = new(new Uri(API_CONFIGURATION_URI), new DefaultAzureCredential());
        }

        public string this[string key]
        {
            get
            {
                return client.GetConfigurationSetting(key).Value.Value;
            }
            set
            {
                client.SetConfigurationSetting(new(key, value));
            }
        }

        public void RemoveSetting(string key)
        {
            client.DeleteConfigurationSetting(key);
        }
    }
}
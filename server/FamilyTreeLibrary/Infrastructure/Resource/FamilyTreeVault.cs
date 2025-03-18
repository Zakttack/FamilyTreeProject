using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FamilyTreeLibrary.Serialization;
using System.Text.Json;
namespace FamilyTreeLibrary.Infrastructure.Resource
{
    public class FamilyTreeVault
    {
        private readonly IReadOnlyDictionary<string,BridgeInstance> secretValuePairs;

        public FamilyTreeVault(FamilyTreeConfiguration configuration)
        {
            secretValuePairs = LoadSecrets(configuration);
        }

        public BridgeInstance this[string secretName]
        {
            get
            {
                return secretValuePairs[secretName];
            }
        }
        
        private IReadOnlyDictionary<string,BridgeInstance> LoadSecrets(FamilyTreeConfiguration configuration)
        {
            Dictionary<string,BridgeInstance> pairs = [];
            SecretClient client = new(new Uri(configuration["KeyVault:Uri"]), new DefaultAzureCredential());
            JsonSerializerOptions options = new()
            {
                Converters =
                {
                    new BridgeSerializer()
                },
                WriteIndented = false
            };
            foreach (SecretProperties properties in client.GetPropertiesOfSecrets())
            {
                string secretName = properties.Name;
                KeyVaultSecret secretValue = client.GetSecret(secretName).Value;
                IBridge instance = JsonSerializer.Deserialize<IBridge>($"\"{secretValue.Value}\"", options) ?? throw new KeyNotFoundException($"{secretName} doesn't have a value.");
                pairs[secretName] = instance.Instance;
            }
            return pairs;
        }
    }
}
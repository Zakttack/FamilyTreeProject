using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using VirtualFamilyMuseumLibrary.ConstantStore.Models;

namespace VirtualFamilyMuseumLibrary.ConstantStore.Repositories
{
    public class FamilyVault(FamilyVaultConfig config) : ISensitiveConstantRepository
    {
        private readonly SecretClient client = new(new Uri(config.Uri), new DefaultAzureCredential());

        public string GetSecret(string name)
        {
            Response<KeyVaultSecret> response = client.GetSecret(name);
            return response.Value.Value;
        }
    }
}
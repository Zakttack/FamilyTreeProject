using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using VirtualFamilyMuseumLibrary.Configuration.Models;

namespace VirtualFamilyMuseumLibrary.Configuration.Repositories
{
    public class FamilyVault(FamilyVaultConfig config) : ISensitiveConstantRepository
    {
        private readonly SecretClient client = new(new Uri(config.Uri), new DefaultAzureCredential());

        public async Task<string> GetSecretAsync(string name)
        {
            Response<KeyVaultSecret> response = await client.GetSecretAsync(name);
            return response.Value.Value;
        }
    }
}
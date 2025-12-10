namespace VirtualFamilyMuseumLibrary.ConstantStore.Models
{
    public sealed record FamilyVaultConfig
    {
        public string Uri {get; init;} = "";

        public override string ToString()
        {
            return $"Family Vault Uri: {Uri}";
        }
    }
}
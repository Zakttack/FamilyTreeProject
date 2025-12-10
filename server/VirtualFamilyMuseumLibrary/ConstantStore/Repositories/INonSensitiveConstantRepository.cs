namespace VirtualFamilyMuseumLibrary.ConstantStore.Repositories
{
    public interface INonSensitiveConstantRepository
    {
        public C? BindSection<C>(string key);
        public string? GetValue(string key);
        
    }
}
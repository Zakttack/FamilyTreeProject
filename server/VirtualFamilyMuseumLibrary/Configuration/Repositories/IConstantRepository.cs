namespace VirtualFamilyMuseumLibrary.Configuration.Repositories
{
    public interface IConstantRepository
    {
        public C? BindSection<C>(string key);
        public string? GetValue(string key);
        
    }
}
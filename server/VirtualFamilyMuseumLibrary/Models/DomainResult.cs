namespace VirtualFamilyMuseumLibrary.Models
{
    public class DomainResult<T>
    {
        public required string Message
        {
            get;
            init;
        }

        public T? Payload
        {
            get;
            init;
        }
    }
}
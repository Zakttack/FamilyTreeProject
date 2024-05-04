namespace FamilyTreeAPI.Models
{
    public class ClientFamilyTreeElement
    {
        public MessageResponse Problem
        {
            get;
            set;
        }
        public IEnumerable<FamilyElement> Output
        {
            get;
            set;
        }
    }
}
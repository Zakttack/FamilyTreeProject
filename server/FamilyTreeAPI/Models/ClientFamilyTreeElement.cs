namespace FamilyTreeAPI.Models
{
    public class ClientFamilyTreeElement
    {
        public MessageResponse Problem
        {
            get;
            set;
        }
        public IEnumerable<FamilyElement> Success
        {
            get;
            set;
        }
    }
}
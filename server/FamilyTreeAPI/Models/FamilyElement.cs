namespace FamilyTreeAPI.Models
{
    public class FamilyElement
    {
        public PersonElement Member
        {
            get;
            set;
        }

        public PersonElement InLaw
        {
            get;
            set;
        }

        public string MarriageDate
        {
            get;
            set;
        }
    }
}
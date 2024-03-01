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

        public override string ToString()
        {
            string memberMessage = $"Member = ({Member})";
            string inLawMessage = "InLaw = " + (InLaw is null ? "unknown" : $"({InLaw})");
            string marriageDateMessage = "Marriage Date = " + (MarriageDate is null ? "unknown" : MarriageDate);
            return $"{memberMessage}; {inLawMessage}; {marriageDateMessage};";
        }
    }
}
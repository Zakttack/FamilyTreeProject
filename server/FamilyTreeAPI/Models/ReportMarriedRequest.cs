namespace FamilyTreeAPI.Models
{
    public class ReportMarriedRequest
    {
        public string MemberName
        {
            get;
            set;
        }

        public string MemberBirthDate
        {
            get;
            set;
        }

        public string MemberDeceasedDate
        {
            get;
            set;
        }

        public string InLawName
        {
            get;
            set;
        }

        public string InLawBirthDate
        {
            get;
            set;
        }

        public string InLawDeceasedDate
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
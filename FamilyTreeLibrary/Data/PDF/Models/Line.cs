using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.PDF.Models
{
    public class Line
    {
        public Line(string name = null, Queue<FamilyTreeDate> dates = default)
        {
            Name = name;
            Dates = dates == default ? new() : dates;
        }
        public string Name
        {
            get;
            set;
        }

        public Queue<FamilyTreeDate> Dates
        {
            get;
        }

        public Line Copy()
        {
            return new(Name,Dates);
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            string result = Name ?? "";
            foreach (FamilyTreeDate d in Dates)
            {
                result += $" {d}";
            }
            return result;
        }
    }
}
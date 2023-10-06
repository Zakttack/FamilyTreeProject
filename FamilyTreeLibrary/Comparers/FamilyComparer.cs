using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Comparers
{
    public class FamilyComparer : IComparer<Family>
    {

        public FamilyComparer(IComparer<DateTime> comparer)
        {
            Comparer = comparer;
        }
        public int Compare(Family a, Family b)
        {
            int birthDateCompare =  Comparer.Compare(a.Member.BirthDate, b.Member.BirthDate);
            return birthDateCompare == 0 ? Comparer.Compare(a.MarriageDate, b.MarriageDate) : birthDateCompare;
        }

        private IComparer<DateTime> Comparer
        {
            get;
        }
    }
}
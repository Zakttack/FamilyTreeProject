using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Comparers
{
    public class FamilyComparer : IComparer<Family>
    {
        public int Compare(Family a, Family b)
        {
            int birthDateCompare =  FamilyTreeUtils.ComparerDate.Compare(a.Member.BirthDate, b.Member.BirthDate);
            return birthDateCompare == 0 ? FamilyTreeUtils.ComparerDate.Compare(a.MarriageDate, b.MarriageDate) : birthDateCompare;
        }
    }
}
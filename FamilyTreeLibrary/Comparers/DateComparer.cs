namespace FamilyTreeLibrary.Comparers
{
    public class DateComparer : IComparer<DateTime>
    {
        public int Compare(DateTime a, DateTime b)
        {
            int yearDiff = a.Year - b.Year;
            int monthDiff = a.Month - b.Month;
            int dayDiff = a.Day - b.Day;
            if (yearDiff == 0)
            {
                if (monthDiff == 0)
                {
                    return dayDiff;
                }
                return monthDiff;
            }
            return yearDiff;
        }
    }
}
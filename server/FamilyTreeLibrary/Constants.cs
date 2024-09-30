using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary
{
    public static class Constants
    {
        public static FamilyTreeDate DefaultDate
        {
            get
            {
                return new()
                {
                    Year = "",
                    Month = "",
                    Day = 0
                };
            }
        }
    }
}
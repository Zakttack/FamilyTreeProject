using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Data.Comparers
{
    public class DirectoryComparer : IComparer<DirectoryInfo>
    {
        public int Compare(DirectoryInfo a, DirectoryInfo b)
        {
            bool aIsNull = a is null || a.FullName is null;
            bool bIsNull = b is null || b.FullName is null;
            if (aIsNull && bIsNull)
            {
                return 0;
            }
            else if (aIsNull && !bIsNull)
            {
                return -1;
            }
            else if (!aIsNull && bIsNull)
            {
                return 1;
            }
            return a.FullName.CompareTo(b.FullName);
        }
    }
}
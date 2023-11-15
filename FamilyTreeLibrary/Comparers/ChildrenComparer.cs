using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Comparers
{
    public class ChildrenComparer : IComparer<IEnumerable<Person>>
    {
        public int Compare(IEnumerable<Person> childrenA, IEnumerable<Person> childrenB)
        {
            Person[] collectionA = childrenA.ToArray();
            Person[] collectionB = childrenB.ToArray();

            int length = Math.Min(collectionA.Length, collectionB.Length);
            for (int i = 0; i < length; i++)
            {
                if (collectionA[i] < collectionB[i])
                {
                    return -1;
                }
                else if (collectionA[i] > collectionB[i])
                {
                    return 1;
                }
            }
            return collectionA.Length - collectionB.Length;
        }
    }
}
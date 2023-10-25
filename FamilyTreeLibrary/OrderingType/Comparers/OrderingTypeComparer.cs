namespace FamilyTreeLibrary.OrderingType.Comparers
{
    public class OrderingTypeComparer : IComparer<AbstractOrderingType[]>
    {
        public int Compare(AbstractOrderingType[] a, AbstractOrderingType[] b)
        {
            for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
            {
                if (a[i].CompareTo(b[i]) < 0)
                {
                    return -1;
                }
                else if (a[i].CompareTo(b[i]) > 0)
                {
                    return 1;
                }
            }
            return a.Length - b.Length;
        }
    }
}
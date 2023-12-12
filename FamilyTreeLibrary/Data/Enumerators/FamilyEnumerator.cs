using FamilyTreeLibrary.Models;
using System.Collections;

namespace FamilyTreeLibrary.Data.Enumerators
{
    public class FamilyEnumerator : IEnumerator<Family>
    {
        private readonly ICollection<ICollection<ICollection<FamilyNode>>> collection;
        private int outerDimensionPosition;
        private int middleDimensionPosition;
        private int innerDimensionPosition;

        public FamilyEnumerator(ICollection<ICollection<ICollection<FamilyNode>>> nodes)
        {
            collection = nodes;
            Reset();
        }

        public Family Current
        {
            get
            {
                Family result = null;
                int tempOuter = outerDimensionPosition;
                int tempMiddle = middleDimensionPosition;
                int tempInner = innerDimensionPosition;
                IReadOnlyList<ICollection<ICollection<FamilyNode>>> outerCollection = collection.ToList();
                if (tempOuter < outerCollection.Count)
                {
                    IReadOnlyList<ICollection<FamilyNode>> middleCollection = outerCollection[tempOuter].ToList();
                    if (tempMiddle < middleCollection.Count)
                    {
                        IReadOnlyList<FamilyNode> innerCollection = middleCollection[tempMiddle].ToList();
                        result = innerCollection[tempInner].Element;
                        if (tempInner == int.MaxValue - 1)
                        {
                            innerDimensionPosition = 0;
                            if (tempMiddle == int.MaxValue - 1)
                            {
                                outerDimensionPosition++;
                                middleDimensionPosition = 0;
                            }
                            else
                            {
                                middleDimensionPosition++;
                            }
                        }
                        else
                        {
                            innerDimensionPosition++;
                        }
                    }
                }
                return result;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        private long CurrentPosition
        {
            get
            {
                checked
                {
                    long outer = (long)(Math.Pow(int.MaxValue, 2) * outerDimensionPosition);
                    long middle = (long)int.MaxValue * middleDimensionPosition;
                    return outer + middle + innerDimensionPosition;
                }
            }
        }

        private long NodesCount
        {
            get
            {
                long count = 0L;
                checked
                {
                    foreach (ICollection<ICollection<FamilyNode>> outer in collection)
                    {
                        foreach (ICollection<FamilyNode> middle in outer)
                        {
                            foreach (FamilyNode inner in middle)
                            {
                                count++;
                            }
                        }
                    }
                }
                return count;
            }
        }

        public void Dispose()
        {
            Reset();
        }

        public bool MoveNext()
        {
            return CurrentPosition < NodesCount;
        }

        public void Reset()
        {
            outerDimensionPosition = 0;
            middleDimensionPosition = 0;
            innerDimensionPosition = 0;
        }
    }
}
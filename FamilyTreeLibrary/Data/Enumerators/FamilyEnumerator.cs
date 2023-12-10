using FamilyTreeLibrary.Models;
using System.Collections;

namespace FamilyTreeLibrary.Data.Enumerators
{
    public class FamilyEnumerator : IEnumerator<Family>
    {
        private readonly IEnumerator<ICollection<FamilyNode>> collectionEnumerator;
        private IEnumerator<FamilyNode> subCollectionEnumerator;

        public FamilyEnumerator(ICollection<ICollection<FamilyNode>> nodes)
        {
            NodeCollection = nodes;
            collectionEnumerator = NodeCollection.GetEnumerator();
            Reset();
        }

        public Family Current
        {
            get
            {
                if (subCollectionEnumerator.MoveNext())
                {
                    return subCollectionEnumerator.Current.Element;
                }
                else if (collectionEnumerator.MoveNext())
                {
                    subCollectionEnumerator = collectionEnumerator.Current.GetEnumerator();
                    return Current;
                }
                return null;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        private ICollection<ICollection<FamilyNode>> NodeCollection
        {
            get;
        }

        public void Dispose()
        {
            Reset();
        }

        public bool MoveNext()
        {
            return subCollectionEnumerator is not null;
        }

        public void Reset()
        {
            collectionEnumerator.Dispose();
            subCollectionEnumerator = collectionEnumerator.MoveNext() ? collectionEnumerator.Current.GetEnumerator() : null;
        }
    }
}
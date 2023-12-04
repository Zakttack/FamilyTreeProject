using FamilyTreeLibrary.Comparers;
using FamilyTreeLibrary.Models;
using MongoDB.Bson;

namespace FamilyTreeLibrary.Data
{
    public class FamilyNode : IComparable<FamilyNode>, IEquatable<FamilyNode>
    {
        public FamilyNode(Family parent, Family element)
        {
            Id = ObjectId.GenerateNewId();
            Parent = parent;
            Element = element;
            Children = new SortedSet<Family>();
        }

        public FamilyNode(BsonDocument document)
        {
            Id = document["_id"].AsObjectId;
            Parent = document[nameof(Parent)].IsBsonNull ? null : new(document[nameof(Parent)].AsBsonDocument);
            Element = document[nameof(Element)].IsBsonNull ? null : new(document[nameof(Element)].AsBsonDocument);
            Children = new SortedSet<Family>();
            BsonArray array = document[nameof(Children)].AsBsonArray;
            foreach (BsonValue value in array)
            {
                Family child = value.IsBsonNull ? null : new(value.AsBsonDocument);
                Children.Add(child);
            }
        }

        public ObjectId Id
        {
            get;
        }

        public Family Parent
        {
            get;
            set;
        }

        public Family Element
        {
            get;
        }

        public ICollection<Family> Children
        {
            get;
        }

        public BsonDocument Document
        {
            get
            {
                Dictionary<string,object> doc = new()
                {
                    {"_id", Id},
                    {nameof(Parent), Parent is null ? BsonNull.Value : Parent.Document},
                    {nameof(Element), Element is null ? BsonNull.Value : Element.Document}
                };
                BsonArray array = new();
                foreach (Family child in Children)
                {
                    array.Add(child is null ? BsonNull.Value : child.Document);
                }
                doc.Add(nameof(Children), array);
                return new(doc);
            }
        }

        public int CompareTo(FamilyNode other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (Parent < other.Parent)
            {
                return -1;
            }
            else if (Parent > other.Parent)
            {
                return 1;
            }
            else if (Element < other.Element)
            {
                return -1;
            }
            else if (Element > other.Element)
            {
                return 1;
            }
            IComparer<IEnumerable<Family>> childrenComparer = new ChildrenComparer();
            return childrenComparer.Compare(Children, other.Children);
        }

        public bool Equals(FamilyNode other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is FamilyNode other && Equals(other);
        }

        public override int GetHashCode()
        {
            int idHash = Id == default ? 0 : Id.GetHashCode();
            int parentHash = Parent is null ? 0 : Parent.GetHashCode();
            int elementHash = Element is null ? 0 : Element.GetHashCode();
            int childrenHash = Children is null ? 0 : Children.GetHashCode();
            return idHash + parentHash + elementHash + childrenHash;
        }

        public override string ToString()
        {
            return Document.ToJson();
        }

        public static bool operator== (FamilyNode a, FamilyNode b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && bIsNull) || (!aIsNull && a.Equals(b));
        }

        public static bool operator!= (FamilyNode a, FamilyNode b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && !bIsNull) || (!aIsNull && !a.Equals(b));
        }

        public static bool operator< (FamilyNode a, FamilyNode b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && !bIsNull) || (!aIsNull && a.CompareTo(b) < 0);
        }

        public static bool operator> (FamilyNode a, FamilyNode b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (!aIsNull && bIsNull) || (!aIsNull && a.CompareTo(b) > 0);
        }
    }
}
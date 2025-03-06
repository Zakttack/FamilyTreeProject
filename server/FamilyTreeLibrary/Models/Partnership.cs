using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Models
{
    public class Partnership : AbstractComparableBridge, IComparable<Partnership>, IEquatable<Partnership>
    {
        private readonly IDictionary<string, BridgeInstance> document;

        public Partnership(IDictionary<string,BridgeInstance> obj, bool needToGenerateId = false)
        {
            if (!obj.ContainsKey("id") && !needToGenerateId)
            {
                throw new UniqueIdentifierNotExistsException("An id must be present to uniquely identify a partnership document.");
            }
            document = obj;
            if (!document.ContainsKey("id"))
            {
                obj["id"] = new(Guid.NewGuid().ToString());
            }
        }

        public Guid Id
        {
            get
            {
                return Guid.Parse(document["id"].AsString);
            }
        }

        public FamilyTreeDate PartnershipDate
        {
            get
            {
                return new(document["partnershipDate"].AsString);
            }
        }

        public BridgeInstance this[string attribute]
        {
            get
            {
                return document[attribute];
            }
            set
            {
                document[attribute] = value;
            }
        }

        public override BridgeInstance Instance => new(document);

        public override int CompareTo(AbstractComparableBridge? other)
        {
            return CompareTo(other as Partnership);
        }

        public int CompareTo(Partnership? p)
        {
            if (p is null)
            {
                return 1;
            }
            return PartnershipDate.CompareTo(p.PartnershipDate);
        }

        public bool Equals(Partnership? other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return PartnershipDate.ToString();
        }
    }
}
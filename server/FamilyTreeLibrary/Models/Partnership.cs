using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Models
{
    public class Partnership : AbstractComparableBridge
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
            if (other is not Partnership p)
            {
                throw new InvalidCastException("Can't compare a partnership to another type.");
            }
            return PartnershipDate.CompareTo(p.PartnershipDate);
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
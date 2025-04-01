using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Serialization;
using System.Text.Json;

namespace FamilyTreeLibrary.Models
{
    public class FamilyDynamic : AbstractComparableBridge, IComparable<FamilyDynamic>, ICopyable<FamilyDynamic>, IEquatable<FamilyDynamic>
    {
        private readonly IDictionary<string, BridgeInstance> document;

        public FamilyDynamic(IDictionary<string,BridgeInstance> obj, bool needToGenerateId = false)
        {
            if (!obj.ContainsKey("id") && !needToGenerateId)
            {
                throw new UniqueIdentifierNotExistsException("An id must be present to uniquely identify a family dynamic document.");
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

        public FamilyTreeDate FamilyDynamicStartDate
        {
            get
            {
                return new(document["familyDynamicStartDate"].AsString);
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
            return CompareTo(other as FamilyDynamic);
        }

        public int CompareTo(FamilyDynamic? p)
        {
            if (p is null)
            {
                return 1;
            }
            return FamilyDynamicStartDate.CompareTo(p.FamilyDynamicStartDate);
        }

        public FamilyDynamic Copy()
        {
            JsonSerializerOptions options = new()
            {
                Converters = {
                    new BridgeSerializer()
                },
                WriteIndented = true
            };
            IBridge bridge = JsonSerializer.Deserialize<IBridge>(Instance.ToString(), options) ?? throw new NullReferenceException("Nothing is there.");
            return new(bridge.Instance.AsObject);
        }

        public bool Equals(FamilyDynamic? other)
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
            return FamilyDynamicStartDate.ToString();
        }
    }
}
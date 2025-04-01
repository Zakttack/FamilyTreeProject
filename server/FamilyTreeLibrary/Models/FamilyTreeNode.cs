using System.Text.Json;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Models
{
    public class FamilyTreeNode : AbstractBridge, ICopyable<FamilyTreeNode>, IEquatable<FamilyTreeNode>
    {
        private readonly IDictionary<string, BridgeInstance> vertex;

        public FamilyTreeNode(IDictionary<string,BridgeInstance> instance, bool needToGenerateId = false)
        {
            if (!instance.ContainsKey("id") && !needToGenerateId)
            {
                throw new UniqueIdentifierNotExistsException("An id must be present to uniquely identify a vertex of a graph.");
            }
            vertex = instance;
            if (!vertex.ContainsKey("id"))
            {
                vertex["id"] = new(Guid.NewGuid().ToString());
            }
            else if (!vertex.TryGetValue("memberId", out BridgeInstance value) || value.IsNull)
            {
                throw new UniqueIdentifierNotExistsException("The vertex in the graph must requires the \"Member Id\" property to reference a document in the person collection.");
            }
        }

        public Guid Id
        {
            get
            {
                return Guid.Parse(vertex["id"].AsString);
            }
        }

        public ISet<string> InheritedFamilyNames
        {
            get
            {
                return vertex["inheritedFamilyNames"].AsArray.Select(element => element.AsString).ToHashSet();
            }
            set
            {
                vertex["inheritedFamilyNames"] = new(value.Select<string,BridgeInstance>(element => new(element)));
            }
        }

        public Guid MemberId
        {
            get
            {
                return Guid.Parse(vertex["memberId"].AsString);
            }
        }

        public Guid? InLawId
        {
            get
            {
                return vertex["inLawId"].TryGetString(out string text) ? Guid.Parse(text) : null;
            }
            set
            {
                vertex["inLawId"] = value is null ? new() : new(value.ToString()!);
            }
        }

        public Guid? DynamicId
        {
            get
            {
                return vertex["dynamicId"].IsNull ? null : Guid.Parse(vertex["dynamicId"].AsString);
            }
            set
            {
                vertex["dynamicId"] = value is null ? new() : new(value.ToString()!);
            }
        }

        public override BridgeInstance Instance => new(vertex);

        public FamilyTreeNode Copy()
        {
            JsonSerializerOptions options = new()
            {
                Converters = {
                    new BridgeSerializer()
                },
                WriteIndented = true
            };
            IBridge bridge = JsonSerializer.Deserialize<IBridge>(ToString(), options) ?? throw new NullReferenceException("Nothing is there.");
            return new(bridge.Instance.AsObject);
        }

        public override bool Equals(AbstractBridge? other)
        {
            return other is FamilyTreeNode node && Equals(node);
        }
        public bool Equals(FamilyTreeNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return MemberId == other.MemberId && InLawId == other.InLawId && DynamicId == other.DynamicId;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using FamilyTreeLibrary.Serialization;

namespace FamilyTreeLibrary.Data.Models
{
    public readonly struct Content(Line header, Queue<string> subContent) : ICopyable<Content>, IEquatable<Content>
    {
        public Line Header
        {
            get => header;
        }

        public Queue<string> SubContent
        {
            get => subContent;
        }

        public Content Copy()
        {
            return new(Header, SubContent);
        }

        public bool Equals(Content other)
        {
            if (Header != other.Header)
            {
                return false;
            }
            BridgeInstance a = new(SubContent.Select((item) => new BridgeInstance(item)));
            BridgeInstance b = new(other.SubContent.Select((item) => new BridgeInstance(item)));
            return a == b;
        }

        public override bool Equals(object? obj)
        {
            return obj is Content other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Header, SubContent);
        }

        public static bool operator==(Content a, Content b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(Content a, Content b)
        {
            return !a.Equals(b);
        }
    }
}
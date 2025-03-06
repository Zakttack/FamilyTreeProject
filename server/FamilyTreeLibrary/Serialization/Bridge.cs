using FamilyTreeLibrary.Serialization.Models;

namespace FamilyTreeLibrary.Serialization
{
    public class Bridge : AbstractBridge, IEquatable<Bridge>
    {
        private readonly BridgeInstance instance;

        public Bridge()
        {
            instance = new();
        }
        public Bridge(IEnumerable<BridgeInstance> array)
        {
            instance = new(array);
        }

        public Bridge(bool value)
        {
            instance = new(value);
        }
        
        public Bridge(Number num)
        {
            instance = new(num);
        }

        public Bridge(string text)
        {
            instance = new(text);
        }

        public Bridge(IDictionary<string,BridgeInstance> obj)
        {
            instance = new(obj);
        }
        
        public override BridgeInstance Instance
        {
            get
            {
                return instance;
            }
        }

        public bool Equals(Bridge? other)
        {
            return base.Equals(other);
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
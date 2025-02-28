namespace FamilyTreeLibrary.Serialization
{
    public abstract class AbstractBridge : IBridge
    {
        public abstract BridgeInstance Instance{ get; }

        public override bool Equals(object? obj)
        {
            return obj is AbstractBridge bridge && Instance == bridge.Instance;
        }

        public override int GetHashCode()
        {
            return Instance.GetHashCode();
        }

        public override string ToString()
        {
            return Instance.ToString();
        }

        public static bool operator ==(AbstractBridge? left, AbstractBridge? right)
        {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool operator !=(AbstractBridge? left, AbstractBridge? right)
        {
            return left is null ? right is not null : !left.Equals(right);
        }
    }
}
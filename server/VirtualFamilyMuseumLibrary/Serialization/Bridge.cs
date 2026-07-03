using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibrary.Serialization
{
    public class Bridge(BridgeValue value) : IBridge
    {

        public BridgeValue Value
        {
            get => value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Bridge other && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator==(Bridge? a, Bridge? b)
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }

        public static bool operator!=(Bridge? a, Bridge? b)
        {
            if (a is null) return b is not null;
            return !a.Equals(b);
        }
    }
}
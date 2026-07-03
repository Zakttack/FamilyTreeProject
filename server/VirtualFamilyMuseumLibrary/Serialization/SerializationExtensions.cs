using System.Text.Json;
using VirtualFamilyMuseumLibrary.Models;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibrary.Serialization
{
    public static class SerializationExtensions
    {
        public static JsonSerializerOptions GetOptions(bool writeIndented)
        {
            return new JsonSerializerOptions()
            {
                Converters = { new BridgeSerializer()},
                WriteIndented = writeIndented
            };
        }

        public static object? DeserializeNull(this IBridge bridge)
        {
            if (bridge.Value.IsNull)
            {
                return null;
            }
            throw new InvalidCastException("The value does exist.");
        }

        public static IBridge SerializeNull(this IBridge bridge)
        {
            return new Bridge(new BridgeValue());
        }

        public static string DeserializeString(this IBridge bridge)
        {
            return (string)bridge.Value;
        }

        public static IBridge SerializeString(this IBridge bridge, string input)
        {
            return new Bridge(input);
        }

        public static int DeserializeInt(this IBridge bridge)
        {
            return (int)(Number)bridge.Value;
        }

        public static IBridge SerializeInt(this IBridge bridge, int input)
        {
            return new Bridge((Number)input);
        }

        public static long DeserializeLong(this IBridge bridge)
        {
            return (long)(Number)bridge.Value;
        }

        public static IBridge SerializeLong(this IBridge bridge, long input)
        {
            return new Bridge((Number)input);
        }

        public static double DeserializeDouble(this IBridge bridge)
        {
            return (double)(Number)bridge.Value;
        }

        public static IBridge SerializeDouble(this IBridge bridge, double input)
        {
            return new Bridge((Number)input);
        }

        public static IDictionary<string,BridgeValue> DeserializeObject(this IBridge bridge)
        {
            return bridge.Value.AsObject;
        }

        public static IBridge SerializeObject(this IBridge bridge, IDictionary<string,BridgeValue> obj)
        {
            return new Bridge(new BridgeValue(obj));
        }

        public static IList<BridgeValue> DeserializeArray(this IBridge bridge)
        {
            return bridge.Value.AsArray;
        }

        public static IBridge SerializeArray(this IBridge bridge, IList<BridgeValue> array)
        {
            return new Bridge(new BridgeValue(array));
        }

        public static Guid DeserializeGuid(this IBridge bridge)
        {
            string text = (string)bridge.Value;
            if (!Guid.TryParse(text, out Guid result))
            {
                throw new InvalidCastException("Not a GUID.");
            }
            return result;
        }

        public static IBridge SerializeGuid(this IBridge bridge, Guid input)
        {
            return new Bridge(input.ToString());
        }

        public static FamilyDate DeserializeDate(this IBridge bridge)
        {
            string text = (string)bridge.Value;
            FamilyDate? result = FamilyDate.GetDate(text);
            if (!result.HasValue)
            {
                throw new InvalidCastException("This text is not a date");
            }
            return result.Value;
        }
    }
}
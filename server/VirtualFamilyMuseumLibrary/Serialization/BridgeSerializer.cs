using System.Text.Json;
using System.Text.Json.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibrary.Serialization
{
    public class BridgeSerializer : JsonConverter<IBridge>
    {
        public override bool HandleNull => true;

        public override IBridge Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return new Bridge(ReadValue(ref reader));
        }

        public override void Write(Utf8JsonWriter writer, IBridge value, JsonSerializerOptions options)
        {
            WriteValue(writer, value.Value);
        }

        private static BridgeValue ReadValue(ref Utf8JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => new BridgeValue(),
                JsonTokenType.String => reader.GetString()!,
                JsonTokenType.Number => new Number(reader.GetDouble()),
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.StartObject => new BridgeValue(ReadObject(ref reader)),
                JsonTokenType.StartArray => new BridgeValue(ReadArray(ref reader)),
                _ => throw new JsonException($"{reader.TokenType} isn't supported.")
            };
        }

        private static IDictionary<string,BridgeValue> ReadObject(ref Utf8JsonReader reader)
        {
            IDictionary<string,BridgeValue> obj = new Dictionary<string,BridgeValue>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("No Property Name found");
                }
                string propertyName = reader.GetString()!;
                reader.Read();
                obj[propertyName] = ReadValue(ref reader);
            }
            return obj;
        }

        private static IList<BridgeValue> ReadArray(ref Utf8JsonReader reader)
        {
            IList<BridgeValue> array = [];
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                array.Add(ReadValue(ref reader));
            }
            return array;
        }

        private static void WriteValue(Utf8JsonWriter writer, BridgeValue value)
        {
            if (value.IsNull)
            {
                writer.WriteNullValue();
            }
            else if (value.TryAsString(out string? text) && text is not null)
            {
                writer.WriteStringValue(text);
            }
            else if (value.TryAsNumber(out Number? number) && number is not null)
            {
                if (number.HasValue && number.Value.TryGetInt(out int n1))
                {
                    writer.WriteNumberValue(n1);
                }
                else if (number.HasValue && number.Value.TryGetLong(out long n2))
                {
                    writer.WriteNumberValue(n2);
                }
                else
                {
                    writer.WriteNumberValue((double)number.Value);
                }
            }
            else if (value.TryAsBool(out bool? result) && result.HasValue)
            {
                writer.WriteBooleanValue(result.Value);
            }
            else if (value.TryAsObject(out IDictionary<string,BridgeValue>? obj) && obj is not null)
            {
                WriteObject(writer, obj);
            }
            else if (value.TryAsArray(out IList<BridgeValue>? array) && array is not null)
            {
                WriteArray(writer, array);
            }
        }

        private static void WriteObject(Utf8JsonWriter writer, IDictionary<string,BridgeValue> obj)
        {
            writer.WriteStartObject();
            foreach (KeyValuePair<string,BridgeValue> pair in obj)
            {
                writer.WritePropertyName(pair.Key);
                WriteValue(writer, pair.Value);
            }
            writer.WriteEndObject();
        }

        private static void WriteArray(Utf8JsonWriter writer, IList<BridgeValue> array)
        {
            writer.WriteStartArray();
            foreach (BridgeValue item in array)
            {
                WriteValue(writer, item);
            }
            writer.WriteEndArray();
        }
    }
}
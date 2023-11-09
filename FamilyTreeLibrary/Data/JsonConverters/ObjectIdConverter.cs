using MongoDB.Bson;
using Newtonsoft.Json;

namespace FamilyTreeLibrary.Data.JsonConverters
{
    public class ObjectIdConverter : JsonConverter<ObjectId>
    {
        public override ObjectId ReadJson(JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value != null ? new ObjectId(reader.Value.ToString()) : ObjectId.Empty;
        }

        public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
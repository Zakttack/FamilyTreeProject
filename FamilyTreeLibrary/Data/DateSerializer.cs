using FamilyTreeLibrary.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace FamilyTreeLibrary.Data
{
    public class DateSerializer : SerializerBase<FamilyTreeDate>
    {
        public DateSerializer()
            :base() {}

        public override FamilyTreeDate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            string value = context.Reader.ReadString();
            return new(value);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, FamilyTreeDate value)
        {
            string output = value.ToString();
            if (output is null || output == string.Empty)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(output);
            }
        }
    }
}
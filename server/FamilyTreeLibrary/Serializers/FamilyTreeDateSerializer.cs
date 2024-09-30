using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FamilyTreeLibrary.Serializers
{
    public class FamilyTreeDateSerializer : JsonConverter<FamilyTreeDate>, IBsonSerializer<FamilyTreeDate>
    {
        public Type ValueType
        {
            get
            {
                return typeof(FamilyTreeDate);
            }
        }

        public FamilyTreeDate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return context.Reader.CurrentBsonType == BsonType.Null ? new FamilyTreeDate(null) : new FamilyTreeDate(context.Reader.ReadString());
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return context.Reader.CurrentBsonType == BsonType.Null ? new FamilyTreeDate(null) : new FamilyTreeDate(context.Reader.ReadString());
        }

        public override FamilyTreeDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType == JsonTokenType.Null ? new FamilyTreeDate(null) : new FamilyTreeDate(reader.GetString());
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, FamilyTreeDate value)
        {
            if (value == Constants.DefaultDate)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(value.ToString());
            }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value is not FamilyTreeDate)
            {
                throw new InvalidDateException(value, DateAttributes.General);
            }
            Serialize(context, args, (FamilyTreeDate)value);
        }

        public override void Write(Utf8JsonWriter writer, FamilyTreeDate value, JsonSerializerOptions options)
        {
            if (value == Constants.DefaultDate)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
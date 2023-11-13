using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            context.Writer.WriteString(value.ToString());
        }
    }
}
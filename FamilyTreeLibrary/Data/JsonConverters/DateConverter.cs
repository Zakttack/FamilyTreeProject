using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;
using Newtonsoft.Json;

namespace FamilyTreeLibrary.Data.JsonConverters
{
    public class DateConverter : JsonConverter<FamilyTreeDate>
    {
        public override FamilyTreeDate ReadJson(JsonReader reader, Type objectType, FamilyTreeDate existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader != null ? new(reader.Value.ToString()) : new(0);
        }

        public override void WriteJson(JsonWriter writer, FamilyTreeDate value, JsonSerializer serializer)
        {
            if (value == new FamilyTreeDate(0))
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Data.JsonConverters
{
    public class ParnetConverter : JsonConverter<Family>
    {
        public override Family ReadJson(JsonReader reader, Type objectType, Family existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader is null || reader.Value is null)
            {
                return null;
            }
            JObject obj = JObject.Load(reader);
            if (obj["Member"] == null)
            {
                return new Family(new Person(string.Empty))
                {
                    InLaw = null,
                    MarriageDate = new(0)
                };
            }
            return new Family(new Person(JObject.Parse(obj["Member"].ToString())))
            {
                InLaw = null,
                MarriageDate = new(0)
            };
        }

        public override void WriteJson(JsonWriter writer, Family value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WriteValue(value.Parent.ToString());
            writer.WriteEndObject();
        }
    }
}
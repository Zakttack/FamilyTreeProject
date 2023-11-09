using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Data.JsonConverters
{
    public class ChildrenConverter : JsonConverter<ICollection<Family>>
    {
        public override ICollection<Family> ReadJson(JsonReader reader, Type objectType, ICollection<Family> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            ICollection<Family> children = new SortedSet<Family>();
            foreach (object item in array)
            {
                children.Add(new Family(new Person(JObject.Parse(item.ToString())))
                {
                    InLaw = null,
                    MarriageDate = new(0)
                });
            }
            return children;
        }

        public override void WriteJson(JsonWriter writer, ICollection<Family> value, JsonSerializer serializer)
        {
            JArray array = new();
            foreach (Family child in value)
            {
                array.Add(child.Member.ToString());
            }
            array.WriteTo(writer);
        }
    }
}
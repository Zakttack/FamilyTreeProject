using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Family
    {
        public Family(Person member, Person inLaw = null, DateTime marriageDate = new(), IEnumerable<Person> children = null)
        {
            Couple = new Person[] {member, inLaw};
            MarriageDate = marriageDate;
            LoadChildren(children);
        }

        public Family(JObject obj)
        {
            Person member = obj["Member"] == null ? null : new(JObject.Parse(obj["Member"].ToString()));
            Person inLaw = obj["InLaw"] == null ? null : new(JObject.Parse(obj["InLaw"].ToString()));
            Couple = new Person[]{member, inLaw};
            MarriageDate = obj["MarriageDate"] == null ? FamilyTreeUtils.DefaultDate : Convert.ToDateTime(JsonConvert.DeserializeObject<string>(obj["MarriageDate"].ToString()));
            ICollection<Person> people = new List<Person>();
            if (obj["Children"] != null)
            {
                JArray array = JArray.Parse(obj["Children"].ToString());
                foreach (JToken token in array)
                {
                    if (token != null)
                    {
                        people.Add(new(JObject.Parse(token.ToString())));
                    }
                }
            }
            LoadChildren(people);
        }

        public Person[] Couple
        {
            get;
        }

        public ICollection<Family> Children
        {
            private set;
            get;
        }

        public DateTime MarriageDate
        {
            get;
            set;
        }

        public JObject FamilyObject
        {
            get
            {
                JObject obj = new()
                {
                    { "Member", JObject.Parse(Couple[0].ToString()) },
                    { "InLaw", JObject.Parse(Couple[1].ToString()) },
                    {"MarriageDate", FamilyTreeUtils.WriteDate(MarriageDate)}
                };
                JArray array = new();
                foreach (Family child in Children)
                {
                    array.Add(JObject.Parse(child.Couple[0].ToString()));
                }
                obj.Add("Children", array);
                return obj;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not Family)
            {
                return false;
            }
            Family other = (Family)obj;
            return ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return FamilyObject.ToString();
        }

        private void LoadChildren(IEnumerable<Person> people)
        {
            Children = new SortedSet<Family>(FamilyTreeUtils.Comparer);
            if (people != null)
            {
                foreach (Person child in people)
                {
                    Children.Add(new(child));
                }
            }
        }
    }
}
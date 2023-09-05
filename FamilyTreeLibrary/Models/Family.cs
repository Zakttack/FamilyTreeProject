using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Family
    {
        public Family(Person member, Person inLaw = null, IEnumerable<Person> children = null)
        {
            Couple = new Person[] {member, inLaw};
            LoadChildren(children);
        }

        public Family(JObject obj)
        {
            Person member = JsonConvert.DeserializeObject<Person>(obj["Member"].ToString());
            Person inLaw = obj["InLaw"] == null ? null : JsonConvert.DeserializeObject<Person>(obj["InLaw"].ToString());
            Couple = new Person[]{member, inLaw};
            IEnumerable<Person> people = obj["Children"] == null ? null : JsonConvert.DeserializeObject<List<Person>>(obj["Children"].ToString());
            LoadChildren(people);
        }

        public Person[] Couple
        {
            get;
        }

        public ICollection<Person> Children
        {
            private set;
            get;
        }

        public JObject FamilyObject
        {
            get
            {
                JObject obj = new()
                {
                    { "Member", JObject.Parse(Couple[0].ToString()) },
                    { "InLaw", JObject.Parse(Couple[1].ToString()) }
                };
                JArray array = new();
                foreach (Person child in Children)
                {
                    array.Add(JObject.Parse(child.ToString()));
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
            Children = new SortedSet<Person>(FamilyTreeUtils.Comparer);
            if (people != null)
            {
                foreach (Person child in people)
                {
                    Children.Add(child);
                }
            }
        }
    }
}
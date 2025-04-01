using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Azure.Cosmos;

namespace FamilyTreeLibrary.Data.Databases
{
    public class PersonCollection : IContainer<Person>
    {
        private readonly Container container;
        private readonly string containerName;

        public PersonCollection(FamilyTreeConfiguration configuration, FamilyTreeVault vault)
        {
            CosmosClient client = new(vault["CosmosDBConnectionString"].AsString, new CosmosClientOptions()
            {
                Serializer = new FamilyTreeDatabaseSerializer(),
                RequestTimeout = TimeSpan.FromMinutes(2),
                ConnectionMode = ConnectionMode.Direct
            });
            containerName = configuration["CosmosDB:PersonContainerName"];
            Database database = client.GetDatabase(configuration["CosmosDB:DatabaseName"]);
            container = database.GetContainer(containerName);
        }

        public Person this[Guid id]
        {
            get
            {
                string query = $"SELECT * FROM {containerName} p WHERE p.id = @id";
                QueryDefinition definition = new QueryDefinition(query)
                    .WithParameter("@id", new Bridge(id.ToString()));
                using FeedIterator<Person> feed = container.GetItemQueryIterator<Person>(definition);
                FeedResponse<Person> response = feed.ReadNextAsync().Result;
                return response.Resource.First();
            }
            set
            {
                FindPerson(value, out Guid actualId);
                if (value.Id != actualId)
                {
                    IDictionary<string,BridgeInstance> obj = new Dictionary<string,BridgeInstance>(value.Instance.AsObject)
                    {
                        ["id"] = new(actualId.ToString())
                    };
                    Person p = new(obj);
                    container.UpsertItemAsync(p, new PartitionKey(value.BirthName)).Wait();
                }
                else
                {
                    container.UpsertItemAsync(value, new PartitionKey(value.BirthName)).Wait();
                }
            }
        }

        public void Remove(Person person)
        {
            container.DeleteItemAsync<Person>(person.Id.ToString(), new PartitionKey(person.BirthName)).Wait();
        }

        private void FindPerson(Person person, out Guid id)
        {
            string query = $"SELECT VALUE p.id FROM {containerName} p WHERE p.birthName = @birthName AND p.birthDate = @birthDate AND p.deceasedDate = @deceasedDate";
            QueryDefinition definition = new QueryDefinition(query)
                .WithParameter("@birthName", new Bridge(person.BirthName))
                .WithParameter("@birthDate", person.BirthDate is null ? new Bridge() : person.BirthDate)
                .WithParameter("@deceasedDate", person.DeceasedDate is null ? new Bridge() : person.DeceasedDate);
            using FeedIterator<Person> feed = container.GetItemQueryIterator<Person>(definition);
            if (feed.HasMoreResults)
            {
                FeedResponse<Person> response = feed.ReadNextAsync().Result;
                IEnumerable<Person> results  = response.Resource;
                int count = results.Count();
                if (count > 1)
                {
                    throw new UniquenessViolationException("There can't be duplicate people.");
                }
                else if (count == 1)
                {
                    id = results.First().Id;
                }
                else
                {
                    id = person.Id;
                }
            }
            else
            {
                id = person.Id;
            }
        }
    }
}
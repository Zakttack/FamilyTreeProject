using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Azure.Cosmos;

namespace FamilyTreeLibrary.Data.Databases
{
    public class FamilyDynamicCollection : IContainer<FamilyDynamic>
    {
        private readonly string containerName;
        private readonly Container container;

        public FamilyDynamicCollection(FamilyTreeConfiguration configuration, FamilyTreeVault vault)
        {
            containerName = configuration["CosmosDB:FamilyDynamicContainerName"];
            CosmosClient client = new(vault["CosmosDBConnectionString"].AsString, new()
            {
                Serializer = new FamilyTreeDatabaseSerializer(),
                RequestTimeout = TimeSpan.FromMinutes(2),
                ConnectionMode = ConnectionMode.Direct
            });
            Database database = client.GetDatabase(configuration["CosmosDB:FamilyDynamicContainerName"]);
            container = database.GetContainer(containerName);
        }

        public FamilyDynamic this[Guid id]
        {
            get
            {
                string query = $"SELECT * FROM {containerName} f WHERE f.id = @id";
                QueryDefinition queryDefinition = new QueryDefinition(query)
                    .WithParameter("@id", id.ToString());
                using FeedIterator<FamilyDynamic> feed = container.GetItemQueryIterator<FamilyDynamic>(queryDefinition);
                FeedResponse<FamilyDynamic> response = feed.ReadNextAsync().Result;
                IEnumerable<FamilyDynamic> familyDynamics = response.Resource;
                return familyDynamics.First();
            }
            set
            {
                if (value.Id != id)
                {
                    FamilyDynamic familyDynamic = new(new Dictionary<string, BridgeInstance>(value.Instance.AsObject)
                    {
                        ["id"] = new(id.ToString())
                    });
                    container.UpsertItemAsync(familyDynamic, new PartitionKey(value.FamilyDynamicStartDate.ToString())).Wait();
                }
                else
                {
                    container.UpsertItemAsync(value, new PartitionKey(value.FamilyDynamicStartDate.ToString())).Wait();
                }
            }
        }

        public void Remove(FamilyDynamic familyDynamic)
        {
            container.DeleteItemAsync<FamilyDynamic>(familyDynamic.Id.ToString(), new PartitionKey(familyDynamic.FamilyDynamicStartDate.ToString())).Wait();
        }
    }
}
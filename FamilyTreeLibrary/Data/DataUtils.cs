using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace FamilyTreeLibrary.Data
{
    public static class DataUtils
    {
        private const string APP_SETTINGS_FILE_NAME = "appsettings.json";

        public static IMongoCollection<BsonDocument> GetCollection(string familyName)
        {
            string appSettingsFilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), APP_SETTINGS_FILE_NAME);
            IConfiguration configuration = FamilyTreeUtils.GetConfiguration(appSettingsFilePath);
            string connectionString = configuration.GetSection("FamilyTreeDb:ConnectionString").Value;
            IMongoClient client = new MongoClient(connectionString);
            string databaseName = configuration.GetSection("FamilyTreeDb:DatabaseName").Value;
            IMongoDatabase database = client.GetDatabase(databaseName);
            ListCollectionNamesOptions options = new()
            {
                Filter = new BsonDocument("name", familyName)
            };
            if (!database.ListCollectionNames(options).ToList().Contains(familyName))
            {
                database.CreateCollection(familyName);
            }
            return database.GetCollection<BsonDocument>(familyName);
        }

        public static IEnumerable<FamilyNode> GetChildrenOf(FamilyNode node, IMongoCollection<BsonDocument> collection)
        {
            ICollection<FamilyNode> children = new SortedSet<FamilyNode>();
            if (node is not null)
            {
                BsonDocument parent = node.Document;
                IEnumerable<BsonDocument> childrenOfParent = parent[nameof(node.Children)].AsBsonArray.Select(doc => doc.AsBsonDocument);
                FilterDefinition<BsonDocument> childrenFilter = Builders<BsonDocument>.Filter.In(nameof(node.Element), childrenOfParent);
                IFindFluent<BsonDocument,BsonDocument> childrenResults = collection.Find(childrenFilter);
                IEnumerable<BsonDocument> records = childrenResults.ToEnumerable().Where(doc => {
                    return doc[nameof(node.Parent)] == parent[nameof(node.Element)];
                });
                foreach (BsonDocument record in records)
                {
                    children.Add(new(record));
                }
            }
            return children;
        }

        public static FamilyNode GetParentOf(FamilyNode node, IMongoCollection<BsonDocument> collection)
        {
            if (node is not null)
            {
                BsonDocument child = node.Document;
                if (child[nameof(node.Parent)] is not null && child[nameof(node.Parent)] != BsonNull.Value)
                {
                    BsonDocument parent = child[nameof(node.Parent)].AsBsonDocument;
                    FilterDefinition<BsonDocument> parentFilter = Builders<BsonDocument>.Filter.Eq(nameof(node.Element), parent);
                    IFindFluent<BsonDocument,BsonDocument> parentResults = collection.Find(parentFilter);
                    IEnumerable<BsonDocument> records = parentResults.ToEnumerable();
                    foreach (BsonDocument record in records)
                    {
                        FamilyNode parentNode = new(record);
                        if (parentNode.Children.Contains(node.Element))
                        {
                            return parentNode;
                        }
                    }
                }
            }
            return null;
        }
    }
}
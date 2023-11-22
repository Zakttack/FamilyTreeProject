using FamilyTreeLibrary.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

        public static IEnumerable<Family> GetChildrenOf(Family node, IMongoCollection<BsonDocument> collection)
        {
            ICollection<Family> children = new SortedSet<Family>();
            if (node is not null)
            {
                BsonDocument document = node.Document;
                IEnumerable<BsonDocument> childrenDocs = document["Children"].AsBsonArray
                    .Select(child => child.AsBsonDocument);
                FilterDefinition<BsonDocument> personToChildFilter = Builders<BsonDocument>.Filter.In("Member", childrenDocs);
                IFindFluent<BsonDocument,BsonDocument> personToChildResult = collection.Find(personToChildFilter);
                IEnumerable<BsonDocument> matches = personToChildResult.ToEnumerable().Where((doc) => {
                    return !doc["Parent"].IsBsonNull && doc["Parent"] == document["Member"];
                });
                foreach (BsonDocument doc in matches)
                {
                    children.Add(new(doc));
                }
            }
            return children;
        }

        public static Family GetParentOf(Family node, IMongoCollection<BsonDocument> collection)
        {
            if (node is not null)
            {
                BsonDocument document = node.Document;
                if (!document["Parent"].IsBsonNull)
                {
                    BsonDocument parentDoc = document["Parent"].AsBsonDocument;
                    FilterDefinition<BsonDocument> personToParentFilter = Builders<BsonDocument>.Filter.Eq("Member", parentDoc);
                    IFindFluent<BsonDocument,BsonDocument> personToParentResult = collection.Find(personToParentFilter);
                    IEnumerable<BsonDocument> parentPossibilities = personToParentResult.ToEnumerable();
                    foreach (BsonDocument possibility in parentPossibilities)
                    {
                        Family fam = new(possibility);
                        if (fam.Children.Contains(node.Member))
                        {
                            return fam;
                        }
                    }
                }
            }
            return null;
        }
    }
}
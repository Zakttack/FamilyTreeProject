using FamilyTreeLibrary.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace FamilyTreeLibrary.Data
{
    public static class DataUtils
    {
        private const string APP_SETTINGS_RELATIVE_FILE_PATH = "appsettings.json";

        public static IMongoCollection<BsonDocument> GetCollection(string familyName)
        {
            string appSettingsFilePath = FamilyTreeUtils.GetFilePathOf(APP_SETTINGS_RELATIVE_FILE_PATH);
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
                IEnumerable<BsonDocument> childrenOfParent = parent[3].AsBsonArray.Select(doc => doc.AsBsonDocument);
                FilterDefinition<BsonDocument> childrenFilter = Builders<BsonDocument>.Filter.In("Element", childrenOfParent);
                using IAsyncCursor<BsonDocument> cursor = collection.Find(childrenFilter).ToCursor();
                IEnumerable<BsonDocument> records = cursor.ToEnumerable().Where(doc => doc[1] == parent[2]);
                foreach (BsonDocument record in records)
                {
                    children.Add(new(record));
                }
            }
            return children;
        }

        public static FamilyNode GetNodeOf(Family element, IMongoCollection<BsonDocument> collection)
        {
            if (element is null)
            {
                return null;
            }
            BsonDocument elementObj = element.Document;
            FilterDefinition<BsonDocument> elementFilter = Builders<BsonDocument>.Filter.Eq("Element", elementObj);
            using IAsyncCursor<BsonDocument> cursor = collection.Find(elementFilter).ToCursor();
            IReadOnlyList<BsonDocument> elementResults = cursor.ToList();
            return elementResults.Any() ? new(elementResults[0]) : null;
        }

        public static IEnumerable<FamilyNode> GetNodesToBeUpdated(Family element, IMongoCollection<BsonDocument> mongoCollection)
        {
            ICollection<FamilyNode> nodes = new List<FamilyNode>(); // The collection of family nodes requiring update
            FilterDefinition<BsonDocument> parentFilter = Builders<BsonDocument>.Filter.ElemMatch("Children", 
            Builders<BsonDocument>.Filter.Eq("Member", element.Member.Document)); // This filter will the find the parent of the element
            IFindFluent<BsonDocument,BsonDocument> parentResult = mongoCollection.Find(parentFilter); // runs the filter finding the parent of the element
            using IAsyncCursor<BsonDocument> parentCursor = parentResult.ToCursor(); // opens access to the found parent
            IEnumerable<BsonDocument> parentElement = parentCursor.ToEnumerable(); // localizes the found parent.
            foreach (BsonDocument parentDoc in parentElement)
            {
                nodes.Add(new(parentDoc)); // the parent node is added to the collection
            }
            FilterDefinition<BsonDocument> elementFilter = Builders<BsonDocument>.Filter.Eq("Element", element.Document); // This filter will find the element in the collection
            IFindFluent<BsonDocument,BsonDocument> elementResult = mongoCollection.Find(elementFilter); // runs the filter finding the element
            using IAsyncCursor<BsonDocument> elementCursor = elementResult.ToCursor(); // open access to the found element
            IEnumerable<BsonDocument> elements = elementCursor.ToEnumerable(); // localizes the found element
            foreach (BsonDocument elementDoc in elements)
            {
                nodes.Add(new(elementDoc)); // the element node is added to the collection
            }
            FilterDefinition<BsonDocument> childrenFilter = Builders<BsonDocument>.Filter.Eq("Parent", element.Document); // This filter will find the children of the element.
            IFindFluent<BsonDocument,BsonDocument> childrenResult = mongoCollection.Find(childrenFilter); // runs the filter finding the element's children.
            using IAsyncCursor<BsonDocument> childrenCursor = childrenResult.ToCursor(); // open access to the found children
            IEnumerable<BsonDocument> childrenCollection = childrenCursor.ToEnumerable(); // localizes the found children
            foreach (BsonDocument doc in childrenCollection)
            {
                nodes.Add(new(doc)); // the children nodes are added to the collection
            }
            return nodes;
        }

        public static FamilyNode GetParentOf(FamilyNode node, IMongoCollection<BsonDocument> collection)
        {
            if (node is not null)
            {
                BsonDocument child = node.Document;
                if (child[1] is not null && child[1] != BsonNull.Value)
                {
                    BsonDocument parent = child[1].AsBsonDocument;
                    FilterDefinition<BsonDocument> parentFilter = Builders<BsonDocument>.Filter.Eq("Element", parent);
                    using IAsyncCursor<BsonDocument> cursor = collection.Find(parentFilter).ToCursor();
                    IEnumerable<BsonDocument> records = cursor.ToEnumerable();
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
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

        public static IMongoCollection<Family> GetCollection(string familyName)
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
            return database.GetCollection<Family>(familyName);
        }

        public static IEnumerable<Family> GetChildrenOf(Family node, IMongoCollection<Family> collection)
        {
            IMongoQueryable<Family> families = collection.AsQueryable();
            if (node is null)
            {
                ICollection<Family> root = new List<Family>();
                if (families.Any())
                {
                    root.Add(families.First());
                }
                return root;
            }
            else if (!families.Where((record) => record == node).Any())
            {
                return null;
            }
            IEnumerable<Family> potentialChildren = families.Where((record) => node.Children.Contains(record.Member)).AsEnumerable();
            return potentialChildren.Where((potentialChild) => potentialChild.Parent == node.Member);
        }

        public static Family GetParentOf(Family node, IMongoCollection<Family> collection)
        {
            IMongoQueryable<Family> families = collection.AsQueryable();
            if (node is null || node.Parent is null || !families.Where((record) => record == node).Any())
            {
                return null;
            }
            IEnumerable<Family> potentialParents = families.Where((record) => record.Member == node.Parent).AsEnumerable();
            return families.Where((parent) => parent.Children.Contains(node.Member)).FirstOrDefault();
        }
    }
}
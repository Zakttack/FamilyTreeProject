using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static Family ParseRecord(BsonDocument record)
        {
            string jsonString = record.ToJson();
            JObject familyObj = JObject.Parse(jsonString);
            return new(familyObj);
        }
    }
}
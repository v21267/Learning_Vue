using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Venvito_Vue.Controllers
{
  static public class VenvitoHelper
  {
    static public SqlConnection CreateDbConnection()
    {
      string connectionSting = Startup.ConnectionString;
      SqlConnection conn = new SqlConnection(connectionSting);
      conn.Open();
      return conn;
    }

    static public IMongoDatabase GetMongoDatabase()
    {
      string connectionString = Startup.MongoConnectionString;
      string databaseName = Startup.MongoDatabase;
      MongoClient client = new MongoClient(connectionString);
      IMongoDatabase db = client.GetDatabase(databaseName);
      return db;
    }

    static public IMongoCollection<MetricsDefinition> GetMongoDefinitionCollection()
    {
      IMongoDatabase db = GetMongoDatabase();
      IMongoCollection<MetricsDefinition> collection = db.GetCollection<MetricsDefinition>("MetricsDefinition");
      CreateUniqueIndex(collection, "IDX_CODE", "code");
      return collection;
    }

    static public IMongoCollection<MetricsData> GetMongoDataCollection()
    {
      IMongoDatabase db = GetMongoDatabase();
      IMongoCollection<MetricsData> collection = db.GetCollection<MetricsData>("MetricsData");
      CreateUniqueIndex(collection, "IDX_DATE_CODE", "date", "code");
      return collection;
    }

    static private void CreateUniqueIndex<TDocument>(IMongoCollection<TDocument> collection, string indexName, params string[] fieldNames)
    {
      List<BsonDocument> list = collection.Indexes.List().ToList<BsonDocument>();
      if (list.Find(index => index["name"] == indexName) == null)
      {
        IndexKeysDefinition<TDocument> indexDefinition = 
          new IndexKeysDefinitionBuilder<TDocument>().Ascending(new StringFieldDefinition<TDocument>(fieldNames[0]));
        CreateIndexOptions options = new CreateIndexOptions() { Name = indexName, Unique = true };
        for (int i = 1; i < fieldNames.Length; i++)
        {
          string fieldName = fieldNames[i];
          StringFieldDefinition<TDocument> field = new StringFieldDefinition<TDocument>(fieldName);
          indexDefinition = indexDefinition.Ascending(field);
        }
        collection.Indexes.CreateOneAsync(indexDefinition, options);
      }

    }

    static public bool UseMongo
    {
      get { return Startup.UseMongo; }
    }

    static public DateTime IntToDate(int date)
    {
      return DateTime.ParseExact(date.ToString(), "yyyyMMdd", null);
    }
  }
}

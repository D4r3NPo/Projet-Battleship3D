

using MongoDB.Driver;

public class MongoDBManager
{
    readonly MongoClient _client;
    readonly IMongoDatabase _database;
    
    public MongoDBManager(string username, string password, string address,string database)
    {
        string connectionString = $"mongodb://{username}:{password}@{address}:{database}/";
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase("myDatabase");
    }
}
using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDBManager
{
    readonly MongoClient _client;
    readonly IMongoDatabase _database;
    readonly IMongoCollection<BsonDocument> _collection;

    public MongoDBManager(string username, string password, string address, string database)
    {
        string connectionString = $"mongodb://{username}:{password}@{address}:{database}/";
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase("myDatabase");
        _collection = _database.GetCollection<BsonDocument>("Gr5_Personnage");
    }

    public async Task AddPlayer(int playerId, int partyId)
    {
        var document = new BsonDocument
        {
            { "Player_ID", playerId },
            { "Partie_ID", partyId },
            { "Position", new BsonDocument { { "x", 0 }, { "y", 0 }, { "z", 0 } } },
            { "Nb_Tir", 0 },
            { "Move_History", new BsonArray { new BsonDocument { { "x", 0 }, { "y", 0 }, { "z", 0 } } } },
            { "Nb_Vaisseau_Destroy", 0 }
        };

        await _collection.InsertOneAsync(document);
    }

    public async Task MovePlayer(int playerId, int partyId, (int x, int y, int z) position)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );

        var update = Builders<BsonDocument>.Update
            .Set("Position", new BsonDocument { { "x", position.x }, { "y", position.y }, { "z", position.z } })
            .Push("Move_History", new BsonDocument { { "x", position.x }, { "y", position.y }, { "z", position.z } });

        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task AddTir(int playerId, int partyId)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );

        var update = Builders<BsonDocument>.Update.Inc("Nb_Tir", 1);

        await _collection.UpdateOneAsync(filter, update);
    }

}

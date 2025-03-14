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

    public void AddPlayer(int playerId, int partyId)
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

        _collection.InsertOne(document);
    }
    
    public void RemovePlayer(int playerId, int partyId)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );
        _collection.DeleteOne(filter);
    }

    public void MovePlayer(int playerId, int partyId, Vector3 position)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );

        var update = Builders<BsonDocument>.Update
            .Set("Position", new BsonDocument { { "x", position.x }, { "y", position.y }, { "z", position.z } })
            .Push("Move_History", new BsonDocument { { "x", position.x }, { "y", position.y }, { "z", position.z } });

        _collection.UpdateOne(filter, update);
    }

    public void Shoot(int playerId, int partyId, Vector3 direction)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );

        var update = Builders<BsonDocument>.Update.Inc("Nb_Tir", 1);

        _collection.UpdateOne(filter, update);
    }

    public List<(string playerName, int score)> GetScores(int partyId)
    {
        List<(string playerName, int score)> scores = [];
        
        var pipeline = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$PlayerID" },
                { "totalDestroyed", new BsonDocument("$sum", "$NbVaisseauDestroy") }
            }),
            new BsonDocument("$sort", new BsonDocument("totalDestroyed", -1))
        };

        List<BsonDocument> results = _collection.Aggregate<BsonDocument>(pipeline).ToList();

        Console.WriteLine(results);
        
        return scores;
    }
}

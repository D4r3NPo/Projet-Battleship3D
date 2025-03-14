using MongoDB.Bson;
using MongoDB.Driver;
using Projet4;

public class MongoDBManager
{
    readonly SQLManager _sql;
    readonly MongoClient _client;
    readonly IMongoDatabase _database;
    readonly IMongoCollection<BsonDocument> _personnages;
    readonly IMongoCollection<BsonDocument> _ships;

    public MongoDBManager(SQLManager sql, string username, string password, string address, string database)
    {
        _sql = sql;
        string connectionString = $"mongodb://{username}:{password}@{address}:{database}/";
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase("USRS6N_2025");
        _personnages = _database.GetCollection<BsonDocument>("Gr5_Personnage");
        _ships = _database.GetCollection<BsonDocument>("Gr5_Ships");
    }

    public void AddPlayer(int playerId, int partyId)
    {
        bool exist = false; // TODO
        if(!exist)
        {
            var defaultPosition = new BsonDocument { { "x", 0 }, { "y", 0 }, { "z", 0 } };
            var document = new BsonDocument
            {
                { "Player_ID", playerId },
                { "Partie_ID", partyId },
                { "Position", defaultPosition },
                { "Nb_Tir", 0 },
                { "Move_History", new BsonArray {defaultPosition} },
                { "Nb_Vaisseau_Destroy", 0 }
            };
            _personnages.InsertOne(document);
        }
    }
    
    //Graou's work here is done
    public Vector3 GetPlayerPosition(int partyId, int playerId)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );
        var playerPositionBson = _personnages.Find(filter).FirstOrDefault().GetValue("Position");
        int x = playerPositionBson["x"].AsInt32;
        int y = playerPositionBson["y"].AsInt32;
        int z = playerPositionBson["z"].AsInt32;
        return new Vector3(x, y, z);
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

        _personnages.UpdateOne(filter, update);
    }

    public bool Shoot(int playerId, int partyId, Vector3 from, Vector3 toward)
    {
        RegisterShootForPlayer(playerId, partyId);
        
        List<List<Vector3>> shipsPositions = GetShipsPositions(partyId);
        Vector3? positionHit = GameManager.HasShootCollide(from, toward, shipsPositions);

        if (positionHit == null)
            return false;

        List<List<Vector3>> shipsToDetroy = [];
        foreach (List<Vector3> shipPositions in shipsPositions)
        {
            shipPositions.Remove(positionHit.Value);
            if (shipPositions.Count == 0)
            {
                ShipKilled(playerId, partyId);
                shipsToDetroy.Append(shipPositions);
            }
        }

        foreach (List<Vector3> ship in shipsToDetroy)
        {
            shipsPositions.Remove(ship);
        }
        
        UpdateShipPosition(partyId, shipsPositions);
        
        return true;
    }

    void ShipKilled(int partyId, int playerId)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );

        var update = Builders<BsonDocument>.Update.Inc("Nb_Vaisseau_Destroy", 1);

        _personnages.UpdateOne(filter, update);
        
        Console.WriteLine("Ship detroy!");
    }

    void RegisterShootForPlayer(int playerId, int partyId)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId),
            Builders<BsonDocument>.Filter.Eq("Player_ID", playerId)
        );

        var update = Builders<BsonDocument>.Update.Inc("Nb_Tir", 1);
        
        _personnages.UpdateOne(filter, update);
    }

    public List<(string playerName, int score)> GetScores(int partyId)
    {
        List<(string playerName, int score)> scores = [];
        
        var pipeline = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$Player_ID" },
                { "totalDestroyed", new BsonDocument("$sum", "$Nb_Vaisseau_Destroy") }
            }),
            new BsonDocument("$sort", new BsonDocument("totalDestroyed", -1))
        };

        List<BsonDocument> results = _personnages.Aggregate<BsonDocument>(pipeline).ToList();

        
        
        foreach (BsonDocument result in results)
        {
            int playerId = result["_id"].AsInt32;
            int score = result["totalDestroyed"].AsInt32;
            string playerName = _sql.GetPlayerName(playerId);
            scores.Add((playerName, score));
        }
        
        return scores;
    }
    
    public void InitShipPosition(int partyId, List<List<Vector3>> positions)
    {
        var document = new BsonDocument
        {
            { "Partie_ID", partyId },
            { "Ships_Position", new BsonArray {Vector3Converter.ConvertListOfListsToBsonArray(positions)} },
        };

        _ships.InsertOne(document);
    }

    public List<List<Vector3>> GetShipsPositions(int partyId)
    {
        // TODO Fix It found a list of list of list in DB : Too deep
        var filter = Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId);
        
        var document = _ships.Find(filter).FirstOrDefault();
        var shipsPositionField = document["Ships_Position"].AsBsonArray;

        return Vector3Converter.ConvertBsonArrayToListOfLists(shipsPositionField);
    }

    public void UpdateShipPosition(int partyId, List<List<Vector3>> positions)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Partie_ID", partyId)
        );

        var update = Builders<BsonDocument>.Update.Inc("Ships_Position", Vector3Converter.ConvertListOfListsToBsonArray(positions));

        _ships.UpdateOne(filter, update);
    }

    //Graou's work is done here
    public int GetShipCount(int partyId)
    {
        return GetShipsPositions(partyId).Count;
    }

    public int GetScore(int partyId, int playerId)
    {
        // TODO
        throw new NotImplementedException();
    }
}

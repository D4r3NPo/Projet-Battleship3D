using MySql.Data.MySqlClient;
using Projet4;

public class SQLManager
{
    readonly MySqlConnection _connection;

    public SQLManager(string server, string port, string database, string user, string password)
    {
        string connectionString = $"server={server};port={port};uid={user};pwd={password};database={database};";

        try
        {
            _connection = new MySqlConnection(connectionString);
            Console.WriteLine("Connecting to the Server...");
            _connection.Open();
            Console.WriteLine("Successfully connect to the Server");
        }
        catch (MySqlException e)
        {
            _connection = null!;
            Console.WriteLine(e.Message);
            Console.WriteLine("Fail to connect to the Server");
        }
    }

    ~SQLManager() => _connection.Close();

    public int AddNewPlayer(string name, int age, string email)
    {
        try
        {
            string query = "INSERT INTO Gr5_joueur (nom, age, mail, victoire, date_inscription) VALUES (@name, @age, @mail, 0, CURDATE());";
            
            MySqlCommand command = new MySqlCommand(query, _connection);
            
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@age", age);
            command.Parameters.AddWithValue("@mail", email);
            
            command.ExecuteNonQuery();
            Console.WriteLine("Your account has been sucessfully created");

        }
        catch (MySqlException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("Failed to create your account");
        }
        
        return GetPlayerId(name, email);
    }

    public int CreateParty(int mapSize)
    {
        string shipsPosition = GameManager.Create3DShips(mapSize);
        Console.WriteLine(shipsPosition);
        try
        {
            string query = "INSERT INTO Gr5_partie (jeu_id, date_debut, ship_position, map_size) VALUES ((SELECT jeu_id FROM Gr5_jeu WHERE nom = 'Battleship3D'),NOW(),@shipsPosition,@mapSize); SELECT LAST_INSERT_ID()";
            
            MySqlCommand command = new MySqlCommand(query, _connection);
            
            command.Parameters.AddWithValue("@shipsPosition", shipsPosition);
            command.Parameters.AddWithValue("@mapSize", mapSize);
            
            using MySqlDataReader dataReader = command.ExecuteReader();
            
            dataReader.Read();
            int partyId = dataReader.GetInt32(0);
            Console.WriteLine("Party has been created");
            return partyId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Failed to create your party");
            throw;
        }
    }

    public int GetPlayerId(string name, string email)
    {
        string safeName = MySqlHelper.EscapeString(name);
        string safeEmail = MySqlHelper.EscapeString(email);

        string query = $"SELECT joueur_id FROM Gr5_joueur WHERE nom = '{safeName}' AND mail = '{safeEmail}';";
        using MySqlDataReader dataReader = new MySqlCommand(query, _connection).ExecuteReader();
        dataReader.Read();
        int playerId = dataReader.GetInt32(0);
        return playerId;
    }

    public List<int> GetPartyList()
    {
        List<int> partyIds = [];
        
        string query = "SELECT partie_id FROM Gr5_partie WHERE state='InProgress'";
        
        using MySqlDataReader result = new MySqlCommand(query, _connection).ExecuteReader();
        
        while(result.Read()) {
            int id = (int)result["partie_id"];
            partyIds.Add(id);
        }
        
        return partyIds;
    }

    public List<Vector3> GetShipsPosition()
    {
        return default;
    }
}
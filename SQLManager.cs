using MySql.Data.MySqlClient;

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
        }
        catch (MySqlException exception)
        {
            Console.WriteLine(exception.Message);
        }
        finally
        {
            Console.WriteLine("Successfully connect to the Server");
        }
    }

    public void CloseConnection()
    {
        _connection.Close();
    }

    public int AddNewPlayer(string name, int age, string email)
    {
        try
        {
            string query = "INSERT INTO Gr5_joueur (nom, age, mail, victoire, date_inscription) VALUES (@name, @age, @mail, 0, CURDATE());";
            
            MySqlCommand command = new MySqlCommand(query, _connection);
            
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@age", age);
            command.Parameters.AddWithValue("@mail", email);
            
            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine("Row affected " + rowsAffected);
        }
        catch (MySqlException exeption)
        {
            Console.WriteLine(exeption.Message);
        }
        finally
        {
            Console.WriteLine("Your account has been sucessfully created");
        }
        
        return GetPlayerId(name, email);
    }

    public int GetPlayerId(string name, string email)
    {
        string safeName = MySqlHelper.EscapeString(name);
        string safeEmail = MySqlHelper.EscapeString(email);

        string query = $"SELECT joueur_id FROM Gr5_joueur WHERE nom = '{safeName}' AND mail = '{safeEmail}';";
        MySqlDataReader dataReader = ExecuteReaderQuery(query);
        
        dataReader.Read();
        int playerId = dataReader.GetInt32(0);
        dataReader.Close();
        
        return playerId;
    }

    MySqlDataReader ExecuteReaderQuery(string query)
    {
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand command = new MySqlCommand(query, _connection);
            reader = command.ExecuteReader();
        }
        catch (MySqlException exeption)
        {
            Console.WriteLine(exeption.Message);
        }
        
        return reader;
    }
}
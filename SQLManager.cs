using MySql.Data.MySqlClient;

public class SQLManager
{
    private string server = "81.1.20.23";
    private string port = "3306";
    private string database = "USRS6N_1";
    private string user = "EtudiantJvd";
    private string password = "!?CnamNAQ01?!";
    
    private string connectionString;
    private MySqlConnection connection;

    public SQLManager()
    {
        connectionString = $"server={server};port={port};uid={user};pwd={password};database={database};";

        try
        {
            connection = new MySqlConnection(connectionString);
            Console.WriteLine("Connecting to the Server...");
            connection.Open();
        }
        catch (MySqlException exeption)
        {
            Console.WriteLine(exeption.Message);
        }
        finally
        {
            Console.WriteLine("Sucessfully connected to the Server");
        }
    }
    
    public void CloseConnection()
    {
        connection.Close();
    }

    public int AddNewPlayer(string name, int age, string email)
    {
        try
        {
            string query = "INSERT INTO Gr5_joueur (nom, age, mail, victoire, date_inscription) VALUES (@name, @age, @mail, 0, CURDATE());";
            
            MySqlCommand command = new MySqlCommand(query, connection);
            
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
        int player_id = dataReader.GetInt32(0);
        dataReader.Close();
        
        return player_id;
    }
    
    private MySqlDataReader ExecuteReaderQuery(string query)
    {
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand command = new MySqlCommand(query, connection);
            reader = command.ExecuteReader();
        }
        catch (MySqlException exeption)
        {
            Console.WriteLine(exeption.Message);
        }
        
        return reader;
    }
}
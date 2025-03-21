// ✅ Ajouter un joueur
// ✅ Créer une partie
// ✅ Initialiser une partie
//      ✅ Créer les vaisseaux
//      ✅ Configurer les vaisseaux
// ✅ Lancer une partie
// ✅ Quitter une partie
// ✅ Reprendre un partie
// ✅ Déroulement d'une partie
//      ✅ Mouvement
//      ✅ Tir
//      ✅ Affichage du classement
// ✅ Timer
// ❌ Vidéo : Tester des parties à 3 joueurs en simultané sur le serveur de l'Enjmin

using System.Text.RegularExpressions;
using static System.Console;
// ReSharper disable AccessToModifiedClosure

const bool withClear = true;

SQLManager sqlDB = new("81.1.20.23", "3306", "USRS6N_1", "EtudiantJvd", "!?CnamNAQ01?!");
MongoDBManager mongoDB = new(sqlDB,"AdminLJV", "!!DBLjv1858**", "81.1.20.23", "27017");
int? playerId = null;
int? partyId = null;
List<int> partyIds = [];

HomeMenu();

return;

void HomeMenu()
{
    string? command = null;
    while (command == null)
    {
        Clear();
        WriteLine("Bienvenue sur Battleship");
        WriteLine("——— Home ———");
        WriteLine("[1] - Se connecter");
        WriteLine("[2] - S'inscrire");
        WriteLine("[q] - Quitter");
        
        switch (command = ReadLine())
        {
            case "1": { Login(); command = null; break; }
            case "2": { Register(); command = null; break; }
            case "q": { Clear(); WriteLine("Quiting..."); break; }
            default: command = null; break;
        }
    }
}

void Register()
{
    Clear();
    WriteLine("——— Register ———");
    string nom = ReadString("Entrer votre nom", "Xx_Graou_xX");
    string email = ReadString("Entrer votre email", "default@email.com");
    int age  = ReadInt("Entrer votre age", 20);
    playerId = sqlDB.AddNewPlayer(nom, age, email);
}

void Login()
{
    Clear();
    WriteLine("——— Login ———");
    string name = ReadString("Entrer votre nom", "Xx_Graou_xX");
    string email = ReadString("Entrer votre email", "default@email.com");
    playerId = sqlDB.GetPlayerId(name, email);
    if (!playerId.HasValue) WriteLine("Failed login. Quitting...");
    while (playerId.HasValue) GameMenu();
}

void GameMenu()
{
    while (playerId.HasValue)
    {
        Clear();
        WriteLine("——— Game ———");
        WriteLine("[1] - Créer une partie");
        WriteLine("[2] - Afficher la liste des parties");
        WriteLine("[3] - Rejoindre une partie");
        WriteLine("[q] - Quitter");
        string command = ReadLine() ?? "Quit";
        switch (command)
        {
            case "1": CreatePartyMenu(); break;
            case "2": DisplayPartyListMenu(); break;
            case "3": JoinPartyMenu(); break;
            case "q": { playerId = null; break; }
        }
    }
}

void CreatePartyMenu()
{
    int? newPartyId = sqlDB.CreateParty(7);
    if (newPartyId.HasValue)
    {
        InitParty(newPartyId.Value);
        JoinParty(newPartyId.Value);
    }
    else WriteLine("Failed create party.");
}

void DisplayPartyListMenu()
{
    Clear();
    partyIds = sqlDB.GetPartyList();
    WriteLine("——— Parties ———");
    foreach (int party in partyIds)
        WriteLine($"Party Id: [{party}]");
    WriteLine("[Any] to quit");
    ReadKey();
}

void JoinPartyMenu()
{
    string? choice = null;
    while (choice == null)
    {
        partyIds = sqlDB.GetPartyList();
        Clear();
        WriteLine("——— Join ———");
        WriteLine("Enter partyId to join :");
        WriteLine("[q] - Quitter]");
        choice = ReadLine();
        if (choice != null)
        {
            if (int.TryParse(choice, out int id) && partyIds.Contains(id))
            {
                partyId = id;
                mongoDB.AddPlayer(playerId.Value, partyId.Value);
                PartyMenu();
            }
        }
    }
}


void InitParty(int partyIdToInit)
{
    WriteLine($"Initializing party {partyIdToInit}...");
    mongoDB.InitShipPosition(partyIdToInit,sqlDB.GetInitialShipsPositions(partyIdToInit));
}

void JoinParty(int partyIdToJoin)
{
    WriteLine($"Joining party {partyIdToJoin}...");
    partyId = partyIdToJoin;
    mongoDB.AddPlayer(playerId.Value, partyId.Value);
    DateTime start = DateTime.Now;
    while ((DateTime.Now - start).TotalMilliseconds < 3000) { }
    PartyMenu();
}

void PartyMenu()
{
    while (partyId.HasValue && playerId.HasValue)
    {
        Vector3 position = mongoDB.GetPlayerPosition(partyId.Value, playerId.Value);
        int shipCount = mongoDB.GetShipCount(partyId.Value);
        int score = mongoDB.GetScore(partyId.Value, playerId.Value);
        Clear();
        WriteLine("——— Party ———");
        WriteLine($"  Location: {position.ToPrettyString()}");
        WriteLine($"     Score: {score}");
        WriteLine($"Ship alive: {shipCount}");
        WriteLine("       [1] - Se déplacer");
        WriteLine("       [2] - Tirer");
        WriteLine("       [3] - Afficher les scores");
        WriteLine("       [q] - Quitter");
        string command = ReadLine() ?? "Quit";
        
        if (IsPartyFinish(partyId.Value))
        {
            WriteLine("——— Party Finish! ———");
            sqlDB.EndParty(partyId.Value);
            DisplayPartyScore(partyId.Value);
            command = "q"; //L'éclaire de génie à 2h du mat
        }
        
        switch (command)
        {
            case "1":
            {
                var destination = ReadMove();
                if (destination.HasValue) mongoDB.MovePlayer(playerId.Value, partyId.Value, destination.Value);
                break;
            }
            case "2":
            {
                var direction = ReadDirection();
                if (direction.HasValue)
                {
                    if (mongoDB.Shoot(playerId.Value, partyId.Value, position, direction.Value)) 
                        WriteLine("Hit");
                    else 
                        WriteLine("Miss");
                }
                break;
            }
            case "3":
            {
                DisplayPartyScore(partyId.Value);
                break;
            }
            case "q": { partyId = null; break; }
        }
    }
}

Vector3? ReadMove()
{
    Clear();
    WriteLine("——— Move ———");
    WriteLine("Choisissez une destination (x,y,z)");
    WriteLine("ou appuyez sur [q] pour annuler");
    return ReadVector3(); // TODO Limit player move
}

Vector3? ReadDirection()
{
    Clear();
    WriteLine("——— Direction ———");
    WriteLine("Choisissez une direction (x,y,z)");
    WriteLine("ou appuyez sur [q] pour annuler");
    return ReadVector3();
}

void DisplayPartyScore(int partyId)
{
    var scores = mongoDB.GetScores(partyId);
    WriteLine("——— Score ———");
    foreach (var scoreEntry in scores) 
        WriteLine($"{scoreEntry.playerName}|{scoreEntry.score}");
}

void Clear()
{
    if (withClear) Console.Clear();
}

string ReadString(string message, string defaultValue)
{
    WriteLine(message);
    string? str = ReadLine();
    if (string.IsNullOrEmpty(str)) str = defaultValue;
    return str;
}

int ReadInt(string message, int defaultValue)
{
    WriteLine(message);
    string? str = ReadLine();
    return string.IsNullOrEmpty(str) ? defaultValue : int.TryParse(str, out int input) ? input : defaultValue;
}

Vector3? ReadVector3()
{
    string? input = null;
    while (input == null)
    {
        input = ReadLine();
        if (input != null)
        {
            if (input == "q") return null;
            const string pattern = @"\(([0-9]+),([0-9]+),([0-9]+)\)";
            if (Regex.IsMatch(input, pattern))
            {
                var match = Regex.Match(input, pattern);
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);
                int z = int.Parse(match.Groups[3].Value);
                return new Vector3(x, y, z);
            }
            input = null;
        }
    }
    return null;
}

bool IsPartyFinish(int partyId)
{
    if (sqlDB.IsTimerFinish(partyId))
    {
        WriteLine("Time's up!");
        return true;
    } 
    else if (mongoDB.GetShipCount(partyId) <= 0)
    {
        WriteLine("All boat have been destroy!");
        return true;
    }
    else
    {
        return false;
    }
}
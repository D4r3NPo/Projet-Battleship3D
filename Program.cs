// ✅ Ajouter un joueur
// ✅ Créer une partie
// TODO Initialiser une partie
//      TODO Créer les vaisseaux
//      TODO Configurer les vaisseaux
// TODO Lancer une partie
// TODO Déroulement d'une partie
//      TODO Mouvement
//      TODO Tir
//      TODO Affichage du classement
// TODO Vidéo : Tester des parties à 3 joueurs en simultané sur le serveur de l'Enjmin

using System.Drawing;
using System.Text.RegularExpressions;
using static System.Console;
// ReSharper disable AccessToModifiedClosure

const bool WithClear =  true;

SQLManager sqlDB = new("81.1.20.23", "3306", "USRS6N_1", "EtudiantJvd", "!?CnamNAQ01?!");
MongoDBManager mongoDB = new("AdminLJV", "!!DBLjv1858**", "81.1.20.23", "27017");
int? playerId;
int? partyId;
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

        command = ReadLine();
        switch (command)
        {
            case "1": { Login(); break; }
            case "2": { Register(); break; }
            case "q":
                {
                    Clear();
                    WriteLine("Quiting...");
                    break;
                }
            default:
                command = null;
                break;
        }
    }


}

void Register()
{
    Clear();
    WriteLine("——— Register ———");
    WriteLine("Entrer votre nom :");
    string? nom = ReadLine();
    if (string.IsNullOrEmpty(nom)) nom = "Xx_Graou_xX";
    
    WriteLine("Entrer votre age :");
    string? ageAsString = ReadLine();
    if (string.IsNullOrEmpty(ageAsString)) ageAsString = "20";
    int age = int.TryParse(ageAsString, out int ageAsInt) ? ageAsInt : 20; 
    
    WriteLine("Entrer votre email :");
    string? email = ReadLine();
    if (string.IsNullOrEmpty(email)) email = "default@email.com";

    playerId = sqlDB.AddNewPlayer(nom, age, email);
}

void Login()
{
    Clear();
    WriteLine("——— Login ———");
    
    WriteLine("Entrer votre nom :");
    string? name = ReadLine();
    if (string.IsNullOrEmpty(name)) name = "Xx_Graou_xX";

    WriteLine("Entrer votre email :");
    string? email = ReadLine();
    if (string.IsNullOrEmpty(email)) email = "default@email.com";
    
    playerId = sqlDB.GetPlayerId(name, email);

    if (playerId.HasValue)
        GameMenu();
    else
        WriteLine("Failed login. Quitting...");
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
            case "1":
                partyId = sqlDB.CreateParty(7);
                if (partyId.HasValue)
                {
                    int cachedPlayerId = playerId.Value;
                    int cachedPartyId = partyId.Value;
                    mongoDB.AddPlayer(playerId.Value, partyId.Value);
                    PartyMenu();
                    mongoDB.RemovePlayer(cachedPlayerId, cachedPartyId);
                }
                break;
            case "2":
                Clear();
                partyIds = sqlDB.GetPartyList();
                WriteLine("——— Parties ———");
                foreach (int party in partyIds)
                    WriteLine($"Party Id: [{party}]");
                WriteLine("[Any] to quit");
                ReadKey();
                break;
            case "3":
                string? choice = null;
                while (choice == null)
                {
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
                            int cachedPlayerId = playerId.Value;
                            int cachedPartyId = partyId.Value;
                            mongoDB.AddPlayer(playerId.Value, partyId.Value);
                            PartyMenu();
                            mongoDB.RemovePlayer(cachedPlayerId, cachedPartyId);
                        }
                    }
                }
                break;
            case "q":
            {
                playerId = null;
                break;
            }
        }
    }
}

void PartyMenu()
{
    while (partyId.HasValue && playerId.HasValue)
    {
        Clear();
        WriteLine("——— Party ———");
        WriteLine("[1] - Se déplacer");
        WriteLine("[2] - Tirer");
        WriteLine("[3] - Afficher les scores");
        WriteLine("[q] - Quitter");
        string command = ReadLine() ?? "Quit";
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
                    if (direction.HasValue) mongoDB.Shoot(playerId.Value, partyId.Value, direction.Value);
                    break;
                }
            case "3":
                {
                    var scores = mongoDB.GetScores(partyId.Value);
                    WriteLine("——— Score ———");
                    foreach (var scoreEntry in scores) 
                        WriteLine($"{scoreEntry.playerName}|{scoreEntry.score}");
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
                return new(x, y, z);
            }

            input = null;
        }
    }
    return null;
}

void Clear()
{
    if (WithClear) Console.Clear();
}



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

using System.Text.RegularExpressions;
using static System.Console;
// ReSharper disable AccessToModifiedClosure

SQLManager sqlDB = new("81.1.20.23", "3306", "USRS6N_1", "EtudiantJvd", "!?CnamNAQ01?!");
MongoDBManager mongoDB = new("AdminLJV", "!!DBLjv1858**", "81.1.20.23", "27017");
int? playerId;
int? partyId;

await HomeMenu();

return;

async Task HomeMenu()
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
            case "1": { await Login(); break; }
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

async Task Login()
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

    if (playerId != 0)
        await GameMenu();
    else
        WriteLine("Failed login. Quitting...");
}

async Task GameMenu()
{
    while (playerId != 0)
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
                {
                    partyId = sqlDB.CreateParty();
                    if (partyId.HasValue)
                    {
                        await PartyMenu();
                    }
                    break;
                }
            case "q": { playerId = null; break; }
        }
    }
}

async Task PartyMenu()
{
    while (partyId.HasValue)
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
                    var destination = Move();
                    if (destination.HasValue)
                    {
                        WriteLine($"Move to {destination}");
                        //await mongoDB.MovePlayer(playerId, partyId, move.Value);
                    }
                    break;
                }
            case "2":
                {
                    throw new NotImplementedException();
                }
            case "3":
                {
                    throw new NotImplementedException();
                }
            case "q": { partyId = null; break; }
        }
    }
}

(int x, int y, int z)? Move()
{
    Clear();
    WriteLine("——— Move ———");
    WriteLine("Choisissez une destination (x,y,z)");
    WriteLine("ou appuyez sur [q] pour annuler");
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
                int x = int.Parse(match.Groups[0].Value);
                int y = int.Parse(match.Groups[1].Value);
                int z = int.Parse(match.Groups[2].Value);
                return (x, y, z);
            }

            input = null;
        }
    }
    return null;
}
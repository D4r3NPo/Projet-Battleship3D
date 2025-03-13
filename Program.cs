// ✅ Ajouter un joueur
// TODO Créer une partie
// TODO Initialiser une partie
//      TODO Créer les vaisseaux
//      TODO Configurer les vaisseaux
// TODO Lancer une partie
// TODO Déroulement d'une partie
//      TODO Mouvement
//      TODO Tir
//      TODO Affichage du classement

using static System.Console;

SqlManager databaseManager = new SqlManager();

int playerId = 0;
        
WriteLine("Bienvenue sur Battleship X Pebble Dabble");
WriteLine("[1] - Se connecter");
WriteLine("[2] - S'inscrire");

string? input = ReadLine();

switch (input)
{
    case "1":
    {
        WriteLine("Entrer votre nom :");
        string name = ReadLine() ?? "Xx_Graou_xX";
        
        WriteLine("Entrer votre email :");
        string email = ReadLine() ?? "default@email.com";

        playerId = databaseManager.GetPlayerId(name, email);
        break;
    }
    case "2":
    {
        WriteLine("Entrer votre nom :");
        string nom = ReadLine() ?? "Xx_Graou_xX";
        WriteLine("Entrer votre age :");
        int age = int.Parse(ReadLine() ?? "20");
        WriteLine("Entrer votre email :");
        string email = ReadLine() ?? "default@email.com";
            
        playerId = databaseManager.AddNewPlayer(nom, age, email);
        break;
    }
    default:
        WriteLine("Tu te crois malin à avoir rentré autre chose que 1 ou 2 ? C'est pour tester le code ? Bah il n’y a pas de boucle si tu trompes, cheh, relance l'app");
        ReadKey();
        break;
}

PlayerManager playerManager = new(playerId);
        
databaseManager.CloseConnection();
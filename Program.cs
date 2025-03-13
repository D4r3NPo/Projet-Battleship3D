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
// TODO Vidéo : Tester des parties à 3 joueurs en simultané sur le serveur de l'Enjmin

using static System.Console;

SQLManager databaseManager = new SQLManager("81.1.20.23", "3306", "USRS6N_1", "EtudiantJvd", "!?CnamNAQ01?!");
MongoDBManager mongoDBManager = new MongoDBManager("AdminLJV","!!DBLjv1858**","81.1.20.23","27017");

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

        
databaseManager.CloseConnection();
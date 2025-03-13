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

using static System.Console;

SQLManager sqlDB = new("81.1.20.23", "3306", "USRS6N_1", "EtudiantJvd", "!?CnamNAQ01?!");
MongoDBManager mongoDB = new("AdminLJV","!!DBLjv1858**","81.1.20.23","27017");
int playerId;
int partyId;

HomeMenu();

return;

void HomeMenu()
{
    Clear();
    WriteLine("Bienvenue sur Battleship");
    WriteLine("——— Home ———");
    WriteLine("    [1] - Se connecter");
    WriteLine("    [2] - S'inscrire");
    WriteLine("[Autre] - Quitter");

    string? command = ReadLine();
        
    switch (command)
    {
        case "1": { Login(); break; }
        case "2": { Register(); break; }
    }
}

void Register()
{
    Clear();
    WriteLine("——— Register ———");
    WriteLine("Entrer votre nom :");
    string nom = ReadLine() ?? "Xx_Graou_xX";
    WriteLine("Entrer votre age :");
    int age = int.Parse(ReadLine() ?? "20");
    WriteLine("Entrer votre email :");
    string email = ReadLine() ?? "default@email.com";
            
    playerId = sqlDB.AddNewPlayer(nom, age, email);
}

void Login()
{
    Clear();
    WriteLine("——— Login ———");
    WriteLine("Entrer votre nom :");
    string name = ReadLine() ?? "Xx_Graou_xX";

    WriteLine("Entrer votre email :");
    string email = ReadLine() ?? "default@email.com";

    playerId = sqlDB.GetPlayerId(name, email);
    
    if (playerId != 0)
        GameMenu();
    else
        WriteLine("Failed login. Quitting...");
}

void GameMenu()
{
    Clear();
    WriteLine("——— Game ———");
    WriteLine("    [1] - Créer un partie");
    WriteLine("[Autre] - Quitter");
    string command = ReadLine()?? "Quit";
    switch (command)
    {
        case "1":
        {
            partyId = sqlDB.CreateParty();
            break;
        }
    }
}


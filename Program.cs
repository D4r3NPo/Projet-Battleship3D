SQLManager databaseManager = new SQLManager();
int player_id = 0;
        
Console.WriteLine("Bienvenue sur Battleship X Pebble Dabble");
Console.WriteLine("[1] - Se connecter");
Console.WriteLine("[2] - S'inscrire");
string input = Console.ReadLine();

if (input == "1")
{
    Console.WriteLine("Entrer votre nom :");
    string name = Console.ReadLine() ?? "Xx_Graou_xX";
    Console.WriteLine("Entrer votre email :");
    string email = Console.ReadLine() ?? "default@email.com";

    player_id = databaseManager.GetPlayerId(name, email);
} 
else if (input == "2")
{
    Console.WriteLine("Entrer votre nom :");
    string nom = Console.ReadLine() ?? "Xx_Graou_xX";
    Console.WriteLine("Entrer votre age :");
    int age = int.Parse(Console.ReadLine() ?? "20");
    Console.WriteLine("Entrer votre email :");
    string email = Console.ReadLine() ?? "default@email.com";
            
    player_id = databaseManager.AddNewPlayer(nom, age, email);
}
else
{
    Console.WriteLine("Tu te crois malin à avoir rentré autre chose que 1 ou 2 ? C'est pour tester le code ? Bah il n’y a pas de boucle si tu trompes, cheh, relance l'app");
    Console.ReadKey();
}

PlayerManager playerManager = new PlayerManager(player_id);
        
databaseManager.CloseConnection();
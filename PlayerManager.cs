
public class PlayerManager
{
    public int id;

    public PlayerManager(int id)
    {
        this.id = id;

        if (id == 0)
        {
           Console.WriteLine("Error with the id given");
        }
        else
        {
            Console.WriteLine("Account connected with the id : " + this.id);
        }
    }
    
    //J'avais dans l'optique de faire ici toute les fonctions qui vont avoir besoin de l'id du joueur, comme lui ajouter un tir par exemple
}
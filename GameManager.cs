using System.Text;

namespace Projet4;

public class GameManager
{
    readonly static Random random = new();
    
    internal class MapData(int size)
    {
        public Dictionary<byte,int> shipHPs = new();
        public readonly byte?[,,] Map = new byte?[size, size, size];
    }
    
    public static string Create3DShips(int mapSize)
    {
        List<Vector3> allVectorBoat = new List<Vector3>();
        
        allVectorBoat.AddRange(CreateSegmentShip(mapSize));
        allVectorBoat.AddRange(CreateSquareShip(mapSize));
        allVectorBoat.AddRange(CreateCubeShip(mapSize));

        return CreateJsonFromVector3List(allVectorBoat);
    }

    static List<Vector3> CreateSegmentShip(int mapSize)
    {
        int axes = random.Next(0, 3);
        int boatSize = random.Next(0, mapSize);
        
        int startPos = random.Next(0, mapSize - (boatSize + 1));
        List<Vector3> vector3List = [];
        
        switch (axes)
        {
            case 0:
            {
                for (int i = startPos; i < startPos + boatSize; i++)
                    vector3List.Add(new Vector3(i, 0, 0));
                break;
            }
            case 1:
            {
                for (int i = startPos; i < startPos + boatSize; i++) 
                    vector3List.Add(new Vector3(0, i, 0));
                break;
            }
            default:
            {
                for (int i = startPos; i < startPos + boatSize; i++) 
                    vector3List.Add(new Vector3(0, 0, i));
                break;
            }
        }
        
        return vector3List;
    }

    static List<Vector3> CreateSquareShip(int mapSize)
    {
        int plane = random.Next(0, 3);
        int squareSize = random.Next(1, mapSize);
        int constant = random.Next(0, mapSize + 1);
        
        int startPos1 = random.Next(0, mapSize - squareSize + 1);
        int startPos2 = random.Next(0, mapSize - squareSize + 1);

        List<Vector3> vector3List = new List<Vector3>();
        
        for (int i = startPos1; i < startPos1 + squareSize; i++)
        {
            for (int j = startPos2; j < startPos2 + squareSize; j++)
            {
                Vector3 point;
                switch (plane)
                {
                    case 0:
                        point = new Vector3(constant, i, j);
                        break;
                    case 1:
                        point = new Vector3(i, constant, j);
                        break;
                    default:
                        point = new Vector3(i, j, constant);
                        break;
                }
                vector3List.Add(point);
            }
        }
        return vector3List;
    }

    static List<Vector3> CreateCubeShip(int mapSize)
    {
        int cubeSize = random.Next(1, mapSize);
        
        int startX = random.Next(0, mapSize - cubeSize + 1);
        int startY = random.Next(0, mapSize - cubeSize + 1);
        int startZ = random.Next(0, mapSize - cubeSize + 1);

        List<Vector3> vector3List = new List<Vector3>();
        
        for (int x = startX; x < startX + cubeSize; x++)
        {
            for (int y = startY; y < startY + cubeSize; y++)
            {
                for (int z = startZ; z < startZ + cubeSize; z++)
                {
                    vector3List.Add(new Vector3(x, y, z));
                }
            }
        }
        return vector3List;
    }

    static string CreateJsonFromVector3List(List<Vector3> vector3List)
    {
        StringBuilder jsonBuilder = new StringBuilder();
        
        jsonBuilder.AppendLine("[");
        
        for (int i = 0; i < vector3List.Count; i++)
        {
            Vector3 vector3 = vector3List[i];
            
            jsonBuilder.Append(vector3.ToString());
            
            if (i < vector3List.Count - 1)
            {
                jsonBuilder.AppendLine(",");
            }
            else
            {
                jsonBuilder.AppendLine();
            }
        }
        
        jsonBuilder.AppendLine("]");

        return jsonBuilder.ToString();
    }
}
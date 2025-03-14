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
        List<List<Vector3>> allVectorBoat = new List<List<Vector3>>();
        
        allVectorBoat.Add(CreateSegmentShip(mapSize));
        allVectorBoat.Add(CreateSquareShip(mapSize));
        allVectorBoat.Add(CreateCubeShip(mapSize));

        return Vector3Converter.CreateJsonFromListOfListVector3(allVectorBoat);
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

        List<Vector3> vector3List = [];
        for (int x = startX; x < startX + cubeSize; x++)
        for (int y = startY; y < startY + cubeSize; y++)
        for (int z = startZ; z < startZ + cubeSize; z++) 
            vector3List.Add(new Vector3(x, y, z));
        
        return vector3List;
    }

    public static Vector3? ShootAsCollide()
    {
        
    } 
}
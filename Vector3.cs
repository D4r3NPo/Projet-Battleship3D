using System.Text.Json;
using System.Text;
using MongoDB.Bson;

public record struct Vector3(int x, int y, int z)
{
    public override string ToString() => $"{{\"x\":{x}, \"y\":{y}, \"z\":{z}}}";
    public string ToPrettyString() => $"({x},{y},{z})";
}

public static class Vector3Converter
{
    public static List<List<Vector3>> ParseJsonToListOfListVector3(string json)
    {
        return JsonSerializer.Deserialize<List<List<Vector3>>>(json);
    }
    
    public static string CreateJsonFromListOfListVector3(List<List<Vector3>> listOfLists)
    {
        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.AppendLine("[");
        
        for (int i = 0; i < listOfLists.Count; i++)
        {
            List<Vector3> innerList = listOfLists[i];
            jsonBuilder.AppendLine("  [");
            
            for (int j = 0; j < innerList.Count; j++)
            {
                Vector3 vector = innerList[j];
                jsonBuilder.Append("    " + vector.ToString());
                
                if (j < innerList.Count - 1)
                    jsonBuilder.AppendLine(",");
                else
                    jsonBuilder.AppendLine();
            }
        
            jsonBuilder.Append("  ]");

            if (i < listOfLists.Count - 1)
                jsonBuilder.AppendLine(",");
            else
                jsonBuilder.AppendLine();
        }
    
        jsonBuilder.AppendLine("]");
        return jsonBuilder.ToString();
    }
    
    public static BsonArray ConvertListOfListsToBsonArray(List<List<Vector3>> listOfVectorLists)
    {
        BsonArray outerArray = new BsonArray();
        
        foreach (var vectorList in listOfVectorLists)
        {
            BsonArray innerArray = new BsonArray();
            
            foreach (var vector in vectorList)
            {
                BsonDocument vectorDoc = new BsonDocument
                {
                    { "x", vector.x },
                    { "y", vector.y },
                    { "z", vector.z }
                };
                innerArray.Add(vectorDoc);
            }
            
            outerArray.Add(innerArray);
        }
        
        return outerArray;
    }
    
    public static List<List<Vector3>> ConvertBsonArrayToListOfLists(BsonArray bsonArray)
    {
        List<List<Vector3>> listOfVectorLists = new List<List<Vector3>>();
        
        foreach (var inner in bsonArray)
        {
            var innerArray = inner.AsBsonArray;
            
            List<Vector3> vectorList = new List<Vector3>();
            
            foreach (var vectorBson in innerArray)
            {
                var vectorDoc = vectorBson.AsBsonDocument;

                int x = vectorDoc["x"].ToInt32();
                int y = vectorDoc["y"].ToInt32();
                int z = vectorDoc["z"].ToInt32();
                
                vectorList.Add(new Vector3(x, y, z));
            }
            
            listOfVectorLists.Add(vectorList);
        }
        
        return listOfVectorLists;
    }
}
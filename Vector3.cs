
public record struct Vector3(float x, float y, float z)
{
    public override string ToString() => $"{{\"x\":{x}, \"y\":{y}, \"z\":{z}}}";
}
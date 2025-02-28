using System.Numerics;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Resources;

public abstract class ResourceBase
{
    public string Directory { get; }
    public string ResourceName { get; }
    public ResourceType ResourceType { get; init; }
    public Texture Texture { get; set; }
    public Vector2 Position { get; set; } = Vector2.Zero;
    public required Vector2 Origin { get; init; } = Vector2.Zero;
    public int Width => Texture.width;
    public int Height => Texture.height;
    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);

    public ResourceBase(string directory, string resourceName)
    {
        Directory = directory;
        ResourceName = resourceName;
    }

    public virtual void Destroy()
    {
        Raylib.UnloadTexture(Texture);
    }
}
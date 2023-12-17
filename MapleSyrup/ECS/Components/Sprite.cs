using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.ECS.Components;

public class Sprite : Component
{
    public Texture2D Texture;
    public Vector2 Position;
    public Vector2 Origin;
    public float Scale;
    public float Rotation;
    public Color Color;
    public SpriteEffects Flipped;
    public float LayerDepth;
    
    public Sprite()
    {
        Type = ComponentType.Sprite;
        Texture = null;
        Position = Vector2.Zero;
        Origin = Vector2.Zero;
        Scale = 1.0f;
        Rotation = 0f;
        Color = Color.White;
        Flipped = SpriteEffects.None;
        LayerDepth = 0f;
        Enabled = true;
    }
    
    public Sprite(Texture2D texture)
    {
        Type = ComponentType.Sprite;
        Texture = texture;
        Position = Vector2.Zero;
        Origin = Vector2.Zero;
        Scale = 1.0f;
        Rotation = 0f;
        Color = Color.White;
        Flipped = SpriteEffects.None;
        LayerDepth = 0f;
        Enabled = true;
    }
}
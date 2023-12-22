using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.ECS.Components;

public class Cloud : Component
{
    public Texture2D Texture;
    public Vector2 Position = Vector2.Zero;
    public Vector2 Velocity = Vector2.Zero;
    public float Speed = 10f;
    public float StartingPosition = 0f;
    public float PositionLimit = 0f;
}
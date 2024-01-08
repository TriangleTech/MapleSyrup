using MapleSyrup.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Graphics;

public class Camera
{
    public Matrix Transform = Matrix.Identity;
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.Zero;
    public Viewport Viewport;
    public float Rotation = 0f;
    public bool EnableCulling = false;

    public Camera(GameContext context)
    {
        Viewport = context.GraphicsDevice.Viewport;
        Origin = new Vector2(Viewport.Width / 2f, Viewport.Height / 2f);
        Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                           Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                           Matrix.CreateRotationZ(Rotation) *
                           Matrix.CreateScale(1f, 1f, 1) *
                           Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        EnableCulling = true; 
    }

    public void UpdateMatrix()
    {
        Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));
        //Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
        //Matrix.CreateRotationZ(Rotation) *
        //Matrix.CreateScale(1f, 1f, 1) *
        //Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
    }
}
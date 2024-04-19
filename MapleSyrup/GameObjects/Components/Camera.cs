using Microsoft.Xna.Framework;

namespace MapleSyrup.GameObjects.Components;

public class Camera
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Origin = Vector2.Zero;
    public float Zoom = 1f;
    public float Rotation = 0f;
    private Matrix _matrix;

    public Matrix GetTransform()
    {
        return _matrix;
    }

    public void UpdateMatrix()
    {
        _matrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                  Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                  Matrix.CreateRotationZ(Rotation) *
                  Matrix.CreateScale(1f, 1f, 1) *
                  Matrix.CreateTranslation(new Vector3(Origin, 0.0f));;
    }
}
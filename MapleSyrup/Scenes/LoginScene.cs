using System.Numerics;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Systems;
using MapleSyrup.Windowing;
using MapleSyrup.ECS.Systems.Hybrid;
using MapleSyrup.Networking.Packets;
using MapleSyrup.Nx;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Scenes;

public class LoginScene : SceneBase
{
    public LoginScene(string sceneName) 
        : base(sceneName)
    {
        Camera = new Camera2D()
        {
            offset = new Vector2(400, 300),
            rotation = 0f,
            target = Vector2.Zero,
            zoom = 1f
        };
    }

    public override void InitSystems()
    {
        var background = new BackgroundAnimation();
        var mapObj = new MapObjAnimation();
        
        // Add any draw systems here
        DrawSystems.Add(background);
        DrawSystems.Add(mapObj);
        
        // Add any update systems here
        UpdateSystems.Add(background);
        UpdateSystems.Add(mapObj);
    }
}
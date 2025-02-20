using System.Numerics;
using MapleSyrup.Common.Map;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Systems;
using MapleSyrup.Windowing;
using MapleSyrup.Common.Packets;
using MapleSyrup.ECS.Systems.Map;
using MapleSyrup.Nx;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Scenes;

public class LoginScene : SceneBase
{
    public LoginScene() 
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

    public override void LoadContent(MapleMap map)
    {
        base.LoadContent(map);
    }
}
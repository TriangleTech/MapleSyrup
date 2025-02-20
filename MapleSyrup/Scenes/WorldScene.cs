using System.Numerics;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Systems;
using MapleSyrup.Windowing;
using MapleSyrup.Common.Packets;
using MapleSyrup.ECS.Systems.Map;
using MapleSyrup.Nx;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Scenes;

public class WorldScene : SceneBase
{
    public WorldScene() 
    {
        Camera = new Camera2D()
        {
            offset = Vector2.Zero,
            target = Vector2.Zero,
            rotation = 0f,
            zoom = 1.0f
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
using System.Numerics;
using Client.ECS;
using Client.ECS.Systems;
using Client.ECS.Systems.Hybrid;
using Client.Nx;
using Client.Windowing;
using ZeroElectric.Vinculum;

namespace Client.Scenes;

public class WorldScene : SceneBase
{
    public WorldScene(string sceneName) 
        : base(sceneName)
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

    public override void LoadContent()
    {
        var imgNode = NXFactory.Shared.GetNode(MapleFiles.Map, $"Map/Map{SceneName[0]}/{SceneName}.img") ?? 
                      throw new Exception("Could not find image");
        LoadBackground(MapleFiles.Map, imgNode);
        for (var i = 0; i < 7; ++i)
        {
            LoadObjects(MapleFiles.Map, imgNode, i);
            LoadTiles(MapleFiles.Map, imgNode, i);
        }
    }
}
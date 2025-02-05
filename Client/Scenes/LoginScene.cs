using Client.ECS;
using Client.ECS.Systems;
using Client.ECS.Systems.Hybrid;
using Client.Nx;
using Client.Windowing;

namespace Client.Scenes;

public class LoginScene : SceneBase
{
    public LoginScene(string sceneName) 
        : base(sceneName)
    {
    }

    public override void InitSystems()
    {
        // Add any draw systems here
        DrawSystems.Add(new BackgroundAnimation());
        DrawSystems.Add(new MapObjAnimation());
        
        // Add any update systems here
        UpdateSystems.Add(new BackgroundAnimation());
        UpdateSystems.Add(new MapObjAnimation());
    }

    public override void LoadContent()
    {
        var imgNode = NXFactory.Shared.GetFastImg(MapleFiles.UI, $"{SceneName}.img") ?? 
                      throw new NullReferenceException("Could not find image");
        LoadBackground(MapleFiles.UI, imgNode);
        //LoadObjects(MapleFiles.UI, imgNode);
        //LoadTiles(MapleFiles.UI, imgNode);
    }
}
﻿using System.Numerics;
using Client.ECS;
using Client.ECS.Systems;
using Client.ECS.Systems.Hybrid;
using Client.Nx;
using Client.Windowing;
using ZeroElectric.Vinculum;

namespace Client.Scenes;

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

    public override void LoadContent()
    {
        var imgNode = NXFactory.Shared.GetFastImg(MapleFiles.UI, $"{SceneName}.img") ?? 
                      throw new NullReferenceException("Could not find image");
        LoadBackground(MapleFiles.UI, imgNode);
        for (var i = 0; i < 7; ++i)
        {
            LoadObjects(MapleFiles.UI, imgNode, i);
            LoadTiles(MapleFiles.UI, imgNode, i);
        }
    }
}
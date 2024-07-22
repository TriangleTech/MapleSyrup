using System.Diagnostics;
using System.Numerics;
using Client.Actors;
using Client.Avatar;
using Client.Managers;
using Client.Map;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Scene;

public class World : IActor
{
    private readonly bool _isLogin;
    public Camera2D Camera;
    
    public uint ID { get; set; }
    public string Name { get; set; }
    public int Z { get; set; }
    public bool Visible { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public ActorLayer Layer { get; set; }
    public ActorType ActorType { get; set; } = ActorType.Player;
    public Rectangle Bounds { get; set; }
    public NxNode Node { get; set; }

    public World(bool isLogin)
    {
        _isLogin = isLogin;
        Camera = new Camera2D
        {
            target = Vector2.Zero,
            offset = new Vector2(400, 300),
            zoom = 1.0f
        };
        
    }
    
    
    public void ProcessPacket(PacketResponse response)
    {
        if (_isLogin)
        {
            switch (response.Opcode) 
            {
                
            }
        }
        else
        {
            switch (response.Opcode)
            {
                
            }
        }
    }
    
    public void Update(float frameTime)
    {
        var actor = ServiceLocator.Get<ActorManager>();
        actor.Update(frameTime);
        actor.ValidateActors();
    }

    public void Draw(float frameTime)
    {
        var actor = ServiceLocator.Get<ActorManager>();
        Raylib.BeginMode2D(Camera);
        actor.Draw(frameTime);
        Raylib.EndMode2D();
    }
    
    public void Clear()
    {
    }
}
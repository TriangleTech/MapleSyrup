using System.Numerics;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Actors;

public interface IActor
{
    public uint ID { get; set; }
    public string Name { get; set; }
    public int Z { get; set; }
    public bool Visible { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public ActorLayer Layer { get; set; }
    public ActorType ActorType { get; set; }
    public Rectangle Bounds { get; set; }
    public NxNode Node { get; set; }
    void Draw(float frameTime);
    void Update(float frameTime);
    void Clear();
    void ProcessPacket(PacketResponse response);
}
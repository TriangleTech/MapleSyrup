using MSClient.Actors;
using MSClient.Net;
using MSClient.NX;

namespace MSClient.Map;

public class Obstacle : Actor
{
    public Obstacle(ref NxNode node)
        : base(ref node)
    {
        
    }
    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Draw(float frameTime)
    {
        throw new NotImplementedException();
    }

    public override void Update(float frameTime)
    {
        throw new NotImplementedException();
    }

    public override void ProcessPacket(PacketResponse response)
    {
        throw new NotImplementedException();
    }
}
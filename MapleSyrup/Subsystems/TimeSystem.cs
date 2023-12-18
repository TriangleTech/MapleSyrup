using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using Microsoft.Xna.Framework;

namespace MapleSyrup.Subsystems;

public class TimeSystem : ISubsystem
{
    public GameContext Context { get; private set; }
    public float DeltaTime { get; private set; }
    public float TotalTime { get; private set; }
    
    public void Initialize(GameContext context)
    {
        Context = context;
        Context.SubscribeToEvent(EventType.OnEngineUpdate, new Subscriber() { EventType = EventType.OnEngineUpdate, Sender = this, Event = OnUpdateTime });
    }
    
    private void OnUpdateTime(EventData eventData)
    {
        var gameTime = eventData["GameTime"] as GameTime;
        DeltaTime = (float)gameTime.ElapsedGameTime.Milliseconds;
        TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;
    }

    public void Shutdown()
    {
    }
}
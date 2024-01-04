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
        var events = Context.GetSubsystem<EventSystem>();
        events.Subscribe(this, "ENGINE_UPDATE", OnUpdateTime);
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
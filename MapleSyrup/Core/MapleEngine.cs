using MapleSyrup.Core.Event;
using MapleSyrup.Resources;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Core;

public class MapleEngine : EventObject
{
    public MapleEngine(GameContext context)
        : base(context)
    {
        Context.AddSubsystem<GraphicsSystem>();
        Context.AddSubsystem<ResourceSystem>();
        Context.AddSubsystem<SceneSystem>();
        
        RegisterEvent(EventType.OnEngineInitialized);
        RegisterEvent(EventType.OnEngineRender);
        RegisterEvent(EventType.OnEngineUpdate);
    }

    public void Initialize(ResourceBackend resourceBackend)
    {
        Context.GetSubsystem<ResourceSystem>().SetBackend(resourceBackend);
        
        PublishEvent(EventType.OnEngineInitialized);
    }

    public void Render()
    {
        Context.GraphicsDevice.Clear(Color.Gray);
        PublishEvent(EventType.OnEngineRender);
    }

    public void Update(GameTime gameTime)
    {
        var eventData = new EventData
        {
            ["GameTime"] = gameTime
        };
        PublishEvent(EventType.OnEngineUpdate, eventData);
    }
}
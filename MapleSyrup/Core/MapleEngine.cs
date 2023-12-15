using MapleSyrup.Core.Event;
using MapleSyrup.Resources;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Core;

public class MapleEngine : GameObject
{
    public MapleEngine(GameContext context)
        : base(context)
    {
        Context.AddSubsystem<GraphicsSystem>();
        Context.AddSubsystem<ResourceSystem>();
        RegisterEvent(EventType.BeforeRender);
        RegisterEvent(EventType.RenderPass);
        RegisterEvent(EventType.AfterRender);
        RegisterEvent(EventType.BeforeUpdate);
        RegisterEvent(EventType.UpdatePass);
        RegisterEvent(EventType.AfterUpdate);
    }

    public void Initialize(ResourceBackend resourceBackend)
    {
        Context.GetSubsystem<ResourceSystem>().SetBackend(resourceBackend);
    }

    public void Render()
    {
        Context.GraphicsDevice.Clear(Color.Gray);
        PublishEvent(EventType.BeforeRender, null);
        PublishEvent(EventType.RenderPass, null);
        PublishEvent(EventType.AfterRender, null);
    }

    public void Update(GameTime gameTime)
    {
        PublishEvent(EventType.BeforeUpdate, null);
        var eventData = new EventData
        {
            ["GameTime"] = gameTime
        };
        PublishEvent(EventType.UpdatePass, eventData);
        PublishEvent(EventType.AfterUpdate, null);
    }
}
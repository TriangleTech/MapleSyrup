using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Core;

public class MapleEngine : IGameObject
{
    public GameContext Context { get; }

    public MapleEngine(GameContext context)
    {
        Context = context;
    }

    public void Initialize()
    {
        Context.AddSubsystem<GraphicsSystem>();
        Context.AddSubsystem<NxSystem>();
        Context.AddSubsystem<ResourceSystem>();
        Context.AddSubsystem<NodeSystem>();
        Context.AddSubsystem<WorldSystem>();
    }

    public void Render()
    {
        Context.GraphicsDevice.Clear(Color.Gray);
        Context.SendEvent(EventType.BeforeRender);
        Context.SendEvent(EventType.RenderPass);
        Context.SendEvent(EventType.AfterRender);
    }

    public void Update(GameTime gameTime)
    {
        Context.SendEvent(EventType.BeforeUpdate);
        var eventData = new EventData
        {
            [DataType.GameTime] = gameTime
        };
        Context.SendEvent(EventType.UpdatePass, eventData);
        Context.SendEvent(EventType.AfterUpdate);
    }
}
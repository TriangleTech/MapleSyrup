using MapleSyrup.Core.Event;
using MapleSyrup.Subsystem;
using Microsoft.Xna.Framework;

namespace MapleSyrup.Core;

public class MapleEngine : IMapleObject
{
    public MapleContext Context { get; }

    public MapleEngine(MapleContext context)
    {
        Context = context;
    }

    public void Initialize()
    {
        Context.AddSubsystem<RenderSystem>();
    }

    public void Render()
    {
        Context.SendEvent(EventType.RenderPass);
    }

    public void Update(GameTime gameTime)
    {
        var eventData = new EventData
        {
            [DataType.GameTime] = gameTime
        };

        Context.SendEvent(EventType.UpdatePass, eventData);
    }
}
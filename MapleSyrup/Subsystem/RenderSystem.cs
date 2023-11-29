using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace MapleSyrup.Subsystem;

public sealed class RenderSystem : ISubsystem
{
    public MapleContext Context { get; private set; }

    public void Initialize(MapleContext context)
    {
        Context = context;
        Context.RegisterEventHandler(EventType.RenderPass, OnRender);
    }

    public void Shutdown()
    {
        
    }

    private void OnRender(EventData eventData)
    {
        Context.GraphicsDevice.Clear(Color.Gray);
    }
}
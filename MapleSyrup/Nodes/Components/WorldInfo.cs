using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Subsystems;

namespace MapleSyrup.Nodes.Components;

public class WorldInfo : GameComponent
{
    public string WorldId { get; private set; }
    public bool IsTown { get; private set; }
    public float MobRate { get; private set; }
    public string Bmg { get; private set; }
    public int ReturnMap { get; private set; }
    public bool HideMinimap { get; private set; }
    public bool ForcedReturn { get; private set; }
    public int MoveLimit { get; private set; }
    public string MapMark { get; private set; }
    public int FieldLimit { get; private set; }
    public float VrTop { get; private set; }
    public float VrLeft { get; private set; }
    public float VrBottom { get; private set; }
    public float VrRight { get; private set; }
    
    public WorldInfo(GameContext context) 
        : base(context)
    {
        SubscribeToEvent(EventType.SceneCreated, new Subscriber() { EventType = EventType.SceneCreated, Event = OnLoadInfo });
        SubscribeToEvent(EventType.SceneChanged, new Subscriber() { EventType = EventType.SceneChanged, Event = OnLoadInfo });
    }
    
    private void OnLoadInfo(EventData eventData)
    {
        var scene = eventData["Scene"] as Scene;
        var worldId = eventData["WorldId"] as string;
        if (scene == null || worldId == null)
            return;
        var resource = Context.GetSubsystem<ResourceSystem>();
        
    }
}
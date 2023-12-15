using MapleSyrup.Core;
using MapleSyrup.Core.Event;

namespace MapleSyrup.Nodes;

public class Scene : Node
{
    public Scene(GameContext context) 
        : base(context)
    {
        RegisterEvent(EventType.SceneCreated);
        RegisterEvent(EventType.SceneChanged);
        RegisterEvent(EventType.SceneDestroyed);
        RegisterEvent(EventType.SceneLoaded);
        RegisterEvent(EventType.SceneUnloaded);
        RegisterEvent(EventType.SceneRendered);
        RegisterEvent(EventType.SceneUpdated);
        SubscribeToEvent(EventType.UpdatePass, new Subscriber() { EventType = EventType.UpdatePass, Event = OnUpdate });
    }

    private void OnUpdate(EventData eventData)
    {
        OnSceneUpdated(eventData);
    }

    private void OnSceneCreated(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnSceneChanged(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnSceneDestroyed(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnSceneLoaded(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnSceneUnloaded(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnSceneRendered(EventData eventData)
    {
        throw new NotImplementedException();
    }

    private void OnSceneUpdated(EventData eventData)
    {
        
    }
}
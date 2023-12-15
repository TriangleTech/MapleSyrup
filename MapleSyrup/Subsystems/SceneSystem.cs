using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Nodes;
using MapleSyrup.Nodes.Components;

namespace MapleSyrup.Subsystems;

public class SceneSystem : ISubsystem
{
    public GameContext Context { get; private set;}
    public Scene Current { get; private set; }
    
    public void Initialize(GameContext context)
    {
        Context = context;
    }

    public void Shutdown()
    {
        
    }

    public void LoadScene(string worldId)
    {
        var scene = new Scene(Context);
        scene.AddComponent(new WorldInfo(Context));
        Current = scene;
        
        var eventData = new EventData
        {
            ["Scene"] = scene,
            ["WorldId"] = worldId
        };

        Context.PublishEvent(EventType.SceneCreated, eventData);
    }
    
    public void ChangeScene(string worldId)
    {
        
    }
    
    public void UnloadScene()
    {
        Context.PublishEvent(EventType.SceneUnloaded);
    }
}
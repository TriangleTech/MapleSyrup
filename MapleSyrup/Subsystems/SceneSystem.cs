using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Gameplay;

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
        Current.Shutdown();
    }

    public void LoadScene(string worldId)
    {
        var scene = new Scene(Context);
        scene.LoadScene(worldId);
        Current = scene;
        Context.PublishEvent(EventType.OnSceneCreated);
    }
    
    public void ChangeScene(string worldId)
    {
        
    }
    
    public void UnloadScene()
    {
        Context.PublishEvent(EventType.OnSceneUnloaded);
    }
}
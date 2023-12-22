namespace MapleSyrup.Core.Event;

public enum EventType
{
    // Engine events
    OnInitialize,
    OnShutdown,
    OnUpdate,
    OnRender,
    
    // Scene events
    OnSceneCreated,
    OnSceneChanged,
    OnSceneUnloaded,
    OnSceneUpdate,
    OnSceneRender,
}
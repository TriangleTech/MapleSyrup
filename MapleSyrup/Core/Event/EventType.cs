namespace MapleSyrup.Core.Event;

public enum EventType
{
    // Engine events
    OnEngineInitialized,
    OnEngineShutdown,
    OnEngineUpdate,
    OnEngineRender,
    
    // Scene events
    OnSceneCreated,
    OnSceneChanged,
    OnSceneUnloaded,
    OnSceneUpdate,
    OnSceneRender,
}
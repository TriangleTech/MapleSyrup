namespace MapleSyrup.Core.Event;

public enum EventType
{
    // Engine events
    BeforeRender,
    RenderPass,
    AfterRender,
    BeforeUpdate,
    UpdatePass,
    AfterUpdate,
    
    // Scene events
    SceneCreated,
    SceneChanged,
    SceneDestroyed,
    SceneLoaded,
    SceneUnloaded,
    SceneRendered,
    SceneUpdated,
}
namespace MapleSyrup.Core.Event;

public enum EventType
{
    // Engine Events
    BeforeRender,
    RenderPass,
    AfterRender,
    BeforeUpdate,
    UpdatePass,
    AfterUpdate,
    
    // World Events
    WorldCreated,
    WorldChanged,
    WorldDestroyed,
    
    // Map Events
    MakeBack,
    MakeTile,
    MakeObj,
    
    // Node Events
    NodeCreated,
    NodeChanged,
    NodeDestroyed
}
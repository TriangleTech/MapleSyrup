namespace MapleSyrup.Event;

[Flags]
public enum EventFlag
{
    OnInitialize = 1 << 0,

    OnMapLoaded = 1 << 1,
    OnMapChanged = 1 << 2,
    OnMapUnloaded = 1 << 3,
    
    OnEntityCreated = 1 << 4,
    OnEntityChanged = 1 << 5,
    OnEntityRemoved = 1 << 6,
    OnLayerChanged = 1 << 7,
}
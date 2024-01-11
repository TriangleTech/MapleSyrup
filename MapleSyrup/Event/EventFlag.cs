namespace MapleSyrup.Event;

[Flags]
public enum EventFlag
{
    OnInitialize = 1 << 0,
    OnDataLoad = 1 << 1,
    OnRender = 1 << 2,
    OnUpdate = 1 << 3,

    OnMapLoaded = 1 << 4,
    OnMapChanged = 1 << 5,
    OnMapUnloaded = 1 << 6,
}
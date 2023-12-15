namespace MapleSyrup.Core.Event;

public record EventData
{
    public readonly Dictionary<string, object> Data;
    
    public EventData()
    {
        Data = new();
    }
    
    public object this[string key]
    {
        get => Data[key];
        set => Data[key] = value;
    }
}
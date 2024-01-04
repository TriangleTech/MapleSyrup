namespace MapleSyrup.Core.Event;

/// <summary>
/// Contains the data used to communicate with the subscriber.
/// </summary>
public readonly struct EventData
{
    /// <summary>
    /// Contains the data being transmitted.
    /// </summary>
    private readonly Dictionary<string, object> Data;
    
    public EventData()
    {
        Data = new();
    }
    
    /// <summary>
    /// Obtain or assigned a key with a value.
    /// </summary>
    /// <param name="key"></param>
    public object this[string key]
    {
        get => Data[key];
        set => Data[key] = value;
    }
}
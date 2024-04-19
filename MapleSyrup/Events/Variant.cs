namespace MapleSyrup.Events;

public class Variant
{
    private Dictionary<string, object> _eventData;

    public object this[string dataName]
    {
        get => _eventData[dataName];
        set => _eventData[dataName] = value;
    }
}
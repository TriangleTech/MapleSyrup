namespace MapleSyrup.Core.Event;

public class EventData
{
    private readonly Dictionary<DataType, object> dataMap = new();
    
    /// <summary>
    /// Gets/Sets the value of the data type you're searching.
    /// </summary>
    /// <param name="dataType"></param>
    public object this[DataType dataType]
    {
        get => dataMap[dataType] ?? null;
        set => dataMap[dataType] = value;
    }
}
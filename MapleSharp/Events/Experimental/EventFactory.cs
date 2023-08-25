namespace MapleSharp.Events.Experimental;

public class EventFactory
{
    private Dictionary<string, Func<object, object>> functions = new();
    private Dictionary<string, Action<object>> actions = new();

    public EventFactory()
    {
        
    }
    
    public void RegisterEvent(string name, Action<object> callback)
    {
        if (actions.ContainsKey(name))
        {
            actions[name] += callback;
        }
        else
        {
            actions.Add(name, callback);
        }
    }

    public void RegisterEvent(string name, Func<object, object> callback)
    {
        if (actions.ContainsKey(name))
        {
            functions[name] += callback;
        }
        else
        {
            functions.Add(name, callback);
        }
    }
    
    public void UnregisterEvent(string name)
    {
        if (actions.ContainsKey(name))
        {
            actions.Remove(name);
        }
    }
    
    public void SendEvent(string name, object data)
    {
        if (actions.ContainsKey(name))
        {
            actions[name].Invoke(data);
        }
        
        throw new Exception("[SendEvent] Event does not exist.");
    }
    
    public object RequestEvent(string name, object data)
    {
        if (functions.ContainsKey(name))
        {
            return functions[name].Invoke(data);
        }

        throw new Exception("[RequestEvent] Event does not exist.");
    }
}
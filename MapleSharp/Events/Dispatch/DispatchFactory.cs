using MapleSharp.Events.Dispatch.Interface;

namespace MapleSharp.Events.Dispatch;

public class DispatchFactory
{
    private readonly Dictionary<int, IDispatcher> dispatchers = new();
    private readonly Dictionary<int, IListener> listeners = new();
    
    private Queue<Request> requests = new();
    private Queue<Response> responses = new();
    
    private struct Request
    {
        public DispatchRequest Type;
        public object Data;
    }
    
    private struct Response
    {
        public DispatchResponse Type;
        public object Data;
    }
    
    public void RegisterDispatcher(int id, IDispatcher dispatcher)
    {
        dispatchers.Add(id, dispatcher);
    }
    
    public void RegisterListener(int id, IListener listener)
    {
        listeners.Add(id, listener);
    }
    
    public void UnregisterDispatcher(int id)
    {
        dispatchers.Remove(id);
    }
    
    public void UnregisterListener(int id)
    {
        listeners.Remove(id);
    }
    
    public void DispatchRequest(DispatchRequest type, object data)
    {
        requests.Enqueue(new Request { Type = type, Data = data });
    }
    
    public object DispatchRequest<T>(DispatchRequest type, params object[] data)
    {
        if (data.Length > 1)
        {
            throw new Exception("Too many arguments.");
        }

        foreach (var dispatch in dispatchers.Values)
        {
            if (dispatch.DispatchRequest<T>(type, data) is { } result)
                return (T)result;
        }
        
        Console.WriteLine("[Dispatch Request] No data found.");
        
        return null;
    }
    
    public void DispatchResponse(DispatchResponse type, object data)
    {
        responses.Enqueue(new Response { Type = type, Data = data });
    }
    
    public object DispatchResponse<T>(DispatchResponse type, params object[] data)
    {
        foreach (var listener in listeners.Values)
        {
            if (listener.OnResponse<T>(type, data) is { } result)
                return (T)result;
        }
        Console.WriteLine("[Dispatch Response] No data found.");
        
        return null;
    }
}
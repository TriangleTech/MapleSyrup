namespace MapleSharp.Events.Dispatch.Interface;

public interface IListener
{
    void OnRequest(DispatchRequest type, object data);
    object OnRequest<T>(DispatchRequest type, params object[] data);
    
    void OnResponse(DispatchResponse type, object data);
    object OnResponse<T>(DispatchResponse type, params object[] data);
}
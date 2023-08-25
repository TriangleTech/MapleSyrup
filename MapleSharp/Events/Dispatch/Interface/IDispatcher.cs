namespace MapleSharp.Events.Dispatch.Interface;

public interface IDispatcher
{
    void DispatchRequest(DispatchRequest type, object data);
    object DispatchRequest<T>(DispatchRequest type, params object[] data);
    
    void OnResponse(DispatchResponse type, object data);
    object OnResponse<T>(DispatchResponse type, params object[] data);
}
namespace MapleSharp.Events.Experimental;

public interface IEventHandler
{
    public EventType Type { get; set; }
    public Action<object> OnDispatchData { get; set; }
    public Func<object, object> OnRequestData { get; set; }
}
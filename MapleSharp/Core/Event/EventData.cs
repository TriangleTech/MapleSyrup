using System.ComponentModel.DataAnnotations;

namespace MapleSharp.Core.Event;

public struct EventData
{
    /// <summary>
    /// The name of the event to response to. Leave empty if no response is needed.
    /// </summary>
    public string EventName { get; set; }
    
    /// <summary>
    /// Data to send with the event.
    /// </summary>
    public object Data { get; set; }
    
    /// <summary>
    /// Object that sent the event. Leave null if no response is needed.
    /// </summary>
    public object Sender { get; set; }
    
    public bool IsEmpty => string.IsNullOrEmpty(EventName) && Data == null && Sender == null;
    
    public bool IsDirty { get; set; }
    
    public EventData(string eventName, object sender, [Required]object data)
    {
        EventName = eventName;
        Data = data;
        Sender = sender;
    }
}
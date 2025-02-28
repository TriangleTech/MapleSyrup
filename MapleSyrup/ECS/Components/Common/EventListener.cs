using MapleSyrup.ECS.Interfaces;
using MapleSyrup.Events.Enums;

namespace MapleSyrup.ECS.Components.Common;

public class EventListener : IComponent
{
    public int Owner { get; init; }
    public List<EventType> Events { get; } = new();
}
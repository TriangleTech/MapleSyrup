using MapleSyrup.ECS.Components;

namespace MapleSyrup.ECS;

public class GameObject
{
    public bool IsVisible = true;
    public string Name = "";
    public string Tag = "";
    private List<object> components;
    private List<string> subscribedEvents;
    
    public GameObject()
    {
        components = new () { new TransformComponent() };
        subscribedEvents = new();
    }

    public T GetComponent<T>()
    {
        if (HasComponent<T>())
            return (T)components.Find(x => x is T);
        
        var component = Activator.CreateInstance<T>();
        components.Add(component);

        return (T)components.Find(x => x is T);
    }

    public bool HasComponent<T>()
    {
        return components.Any(x => x is T);
    }

    public void SubscribeToEvent(string eventName)
    {
        if (subscribedEvents.Any(name => name == eventName))
            return;
        subscribedEvents.Add(eventName);
    }

    public void Clear()
    {
        components.Clear();
    }
}
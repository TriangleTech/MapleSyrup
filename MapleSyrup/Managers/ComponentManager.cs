using System.Runtime.InteropServices;
using MapleSyrup.EC;

namespace MapleSyrup.Managers;

public class ComponentManager : IManager
{
    private ManagerLocator _locator;
    private readonly List<IComponent> _components;

    public ComponentManager()
    {
        _components = new();
    }
    
    public void Initialize(ManagerLocator locator)
    {
        _locator = locator;
    }

    public T Create<T>(Entity entity) where T: IComponent
    {
        var component = Activator.CreateInstance<T>();
        if (entity & component)
            return Get<T>(entity);
        
        entity.ComponentFlag |= component.Flag;
        component.Parent = entity;
        _components.Add(component);

        return component;
    }

    public T Get<T>(Entity entity) where T : IComponent
    {
        var component = _components.Find(comp => comp.Parent == entity && comp is T);

        if (!(entity & component))
            return default;

        return (T)component;
    }

    public void Shutdown()
    {
        
    }
}
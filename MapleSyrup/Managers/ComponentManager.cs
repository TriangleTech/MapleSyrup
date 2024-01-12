using System.Runtime.InteropServices;
using MapleSyrup.EC;
using MapleSyrup.EC.Components;

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

    public T Create<T>(IEntity entity) where T: IComponent
    {
        if (_components.Any(x => x.Parent == entity && x is T))
            return Get<T>(entity);
        var component = Activator.CreateInstance(typeof(T), entity) as IComponent;
        entity.CFlags |= component.Flag;
        _components.Add(component);

        return (T)component;
    }

    public T? Get<T>(IEntity entity) where T : IComponent
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
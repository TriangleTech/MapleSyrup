using MapleSyrup.ECS.Entities;
using MapleSyrup.ECS.Systems;
using MapleSyrup.Factories;
using MapleSyrup.Resources;

namespace MapleSyrup.ECS;

public class World : IDisposable
{
    private List<IDrawableSystem> drawableSystems = new();
    private List<IUpdateableSystem> updateableSystems = new();
    private readonly EntityFactory entityFactory;
    private readonly ResourceFactory resourceFactory;
    private int currentId;
    
    private World()
    {
        entityFactory = new EntityFactory();
        resourceFactory = new ResourceFactory(ResourceBackend.Nx);
    }

    public World AddSystem<T>() where T : ISystem
    {
        var system = Activator.CreateInstance<T>();
        system.Initialize(entityFactory);
        
        switch (system)
        {
            case IDrawableSystem drawableSystem:
                drawableSystems.Add(drawableSystem);
                break;
            case IUpdateableSystem updateableSystem:
                updateableSystems.Add(updateableSystem);
                break;
            default:
                throw new Exception("[World] System is not updateable or drawable!");
        }

        return this;
    }

    public World SetWorldId(int id)
    {
        if (id < 0)
            return this;
        currentId = id;

        return this;
    }

    public static World CreateWorld()
    {
        return new World();
    }

    public void Initialize()
    {
        entityFactory.Initialize();
        resourceFactory.Initialize();
    }

    public void Update(float timeDelta)
    {
        foreach (var system in updateableSystems)
            system.Update(entityFactory, timeDelta);
    }

    public void Draw()
    {
        foreach (var system in drawableSystems)
            system.Draw(entityFactory);
    }

    public void Dispose()
    {
        entityFactory.Dispose();
        resourceFactory.Dispose();
    }
}
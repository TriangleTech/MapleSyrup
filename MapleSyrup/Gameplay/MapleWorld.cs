using System.Collections.ObjectModel;
using MapleSyrup.ECS;
using MapleSyrup.ECS.Systems;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay;

public class MapleWorld
{
    private List<GameObject> gameObjects = new();
    private List<IDrawSystem> drawSystems = new();
    private List<IUpdateSystem> updateSystems = new();
    private GraphicsDevice graphicsDevice;

    internal MapleWorld(int worldId, GraphicsDevice device)
    {
        
    }

    public T AddSystem<T>()
    {
        throw new NotImplementedException();
    }

    public T GetSystem<T>()
    {
        throw new NotImplementedException();
    }

    public void RemoveSystem<T>()
    {
        throw new NotImplementedException();
    }

    public GameObject CreateObject()
    {
        throw new NotImplementedException();
    }

    public void DestroyObject(GameObject gameObject)
    {
        throw new NotImplementedException();
    }

    public void Update(float timeDelta)
    {
        updateSystems.ForEach(system => system.Update(timeDelta));
    }

    public void Draw()
    {
        drawSystems.ForEach(system => system.Draw(this));
    }

    public IReadOnlyList<GameObject> GetObjectsWithComponent<T>()
    {
        return gameObjects.FindAll(x => x.HasComponent<T>()).AsReadOnly();
    }

    public void Clear()
    {
        foreach (var obj in gameObjects)
        {
            obj.Clear();
        }
    }
}
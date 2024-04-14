using MapleSyrup.GameObjects;
using MapleSyrup.GameObjects.Components;
using MapleSyrup.Nx;

namespace MapleSyrup.Managers;

public class ActorManager
{
    private SortedSet<Actor> _actors;
    private static ActorManager _instance;

    public static ActorManager Instance => _instance;

    public SortedSet<Actor> Actors => _actors;

    public ActorManager(Application app)
    {
        _instance = this;
        _actors = new();
    }

    public Mob CreateMob(string mobId)
    {
        var mob = new Mob();
        var resource = ResourceManager.Instance;
        var nx = resource["Mob"];

        nx.GetImage($"Mob/{mobId}.img");
        var states = nx.GetChildrenNames();

        foreach (var (stateName, _) in states)
        {
            var state = new State();
            nx.GetDirectory(stateName);

            if (stateName == "info") // TODO: Handle this when it matters.
            {
                
            }
            else
            {
                var frames = nx.GetChildrenNames();
                foreach (var (frame, offset) in frames)
                {
                    if (frame.Length > 3) continue; // TODO: Deal with this later.
                    var i = int.Parse(frame);
                    state.AddFrame(i, nx.GetTexture(offset, resource.GraphicsDevice));
                }
                
                mob.StateMachine.AddState(stateName, state);
            }
        }
        nx.Restore();
        _actors.Add(mob);
        return mob;
    }

    public void Destroy()
    {
        foreach (var actor in _actors)
        {
            actor.Clear();
        }
    }
}
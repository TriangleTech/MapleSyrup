using MapleSharp.Core.Interface;
using NLua;

namespace MapleSharp.Scripting;

public class LuaSystem : ISubsystem
{
    private List<LuaScript> luaInstances = new();
    private Queue<LuaScript> luaQueue = new();
    
    private struct LuaScript
    {
        public string Script { get; set; }
        public Lua LuaInstance { get; set; }
    }
    
    public LuaSystem()
    {
        
    }

    public void Initialize()
    {
        
    }

    public void Update(float timeDelta)
    {
        if (luaQueue.Count > 0)
        {
            var script = luaQueue.Dequeue();
            script.LuaInstance.DoString(script.Script);
            script.LuaInstance.GetFunction("OnStart")?.Call();
            script.LuaInstance.GetFunction("OnUpdate")?.Call(timeDelta);
        }
    }

    public void Shutdown()
    {
        for (int i = 0; i < luaInstances.Count; i++)
        {
            luaInstances[i].LuaInstance.Dispose();
        }
        
        luaInstances.Clear();
        luaQueue.Clear();
    }
    
    public void ExecuteOnce(string script)
    {
        if (!File.Exists(script))
        {
            Console.WriteLine($"[LuaSystem] Script {script} does not exist.");
            return;
        }

        var text = File.ReadAllText(script);
        var lua = new LuaScript() { Script = text, LuaInstance = new Lua() };
        lua.LuaInstance["Global"] = new LuaFunctions();
        luaQueue.Enqueue(lua);
    }
    
    public void Execute(string script)
    {
        throw new NotImplementedException();
    }
}
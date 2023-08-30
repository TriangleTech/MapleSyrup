using MapleSyrup.Core.Interface;
using NLua;

namespace MapleSyrup.Scripting;

/// <summary>
/// The LuaSystem is used to execute Lua scripts.
/// </summary>
public class LuaSystem : ISubsystem
{
    private List<LuaScript> luaInstances = new();
    private Queue<LuaScript> luaQueue = new();
    
    /// <summary>
    /// Contains the Lua instance and the script. OnStartCalled is optional.
    /// </summary>
    public record LuaScript
    {
        public required string Script { get; set; }
        public required Lua LuaInstance { get; set; }
        public bool OnStartCalled { get; set; }
    }
    
    public LuaSystem()
    {
        
    }

    /// <summary>
    /// NOT USED
    /// </summary>
    public void Initialize()
    {
        
    }

    /// <summary>
    /// Executes all scripts in the queue and the list.
    /// </summary>
    /// <param name="timeDelta"></param>
    public void Update(float timeDelta)
    {
        if (luaQueue.Count > 0)
        {
            var script = luaQueue.Dequeue();
            script.LuaInstance.DoString(script.Script);
            script.LuaInstance.GetFunction("RunOnce")?.Call(timeDelta);
        }

        Task.Run(() =>
        {
            for (int i = 0; i < luaInstances.Count; i++)
            {
                luaInstances[i].LuaInstance.DoString(luaInstances[i].Script);
                if (!luaInstances[i].OnStartCalled)
                {
                    luaInstances[i].LuaInstance.GetFunction("OnStart")?.Call();
                    luaInstances[i].OnStartCalled = true;
                }
                luaInstances[i].LuaInstance.GetFunction("OnUpdate")?.Call(timeDelta);
            }
        });
    }

    /// <summary>
    /// Clears all scripts from the queue and the list.
    /// </summary>
    public void Shutdown()
    {
        for (int i = 0; i < luaInstances.Count; i++)
        {
            luaInstances[i].LuaInstance.Dispose();
        }
        
        luaInstances.Clear();
        luaQueue.Clear();
    }
    
    /// <summary>
    /// Script must have a function called RunOnce(float timeDelta). Upon calling this function, the script will be executed once and then removed from the queue.
    /// <code>function RunOnce(timeDelta)
    ///     -- code here
    /// end</code>
    /// </summary>
    /// <param name="script">location of the scripts. (i.e. scripts/template.lua).</param>
    public void ExecuteOnce(string script)
    {
        if (!File.Exists(script))
        {
            Console.WriteLine($"[LuaSystem] Script {script} does not exist.");
            return;
        }

        var text = File.ReadAllText(script);
        var lua = new LuaScript() { Script = text, LuaInstance = new Lua() };
        lua.LuaInstance.LoadCLRPackage();
        lua.LuaInstance["Global"] = new LuaGlobals();
        luaQueue.Enqueue(lua);
    }
    
    public void Execute(string script)
    {
        if (!File.Exists(script))
        {
            Console.WriteLine($"[LuaSystem] Script {script} does not exist.");
            return;
        }

        var text = File.ReadAllText(script);
        var lua = new LuaScript() { Script = text, LuaInstance = new Lua() };
        lua.LuaInstance.LoadCLRPackage();
        lua.LuaInstance["Global"] = new LuaGlobals();
        luaInstances.Add(lua);
    }
    
    public LuaScript LoadScript(string script)
    {
        if (!File.Exists(script))
        {
            Console.WriteLine($"[LuaSystem] Script {script} does not exist.");
            return null;
        }

        var text = File.ReadAllText(script);
        var lua = new LuaScript() { Script = text, LuaInstance = new Lua() };
        lua.LuaInstance.LoadCLRPackage();
        lua.LuaInstance["Global"] = new LuaGlobals();
        return lua;
    }
}
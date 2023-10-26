using NLua;

namespace MapleSyrup.Resources.Scripting;

/// <summary>
/// Contains the Lua instance and the script. OnStartCalled is optional.
/// </summary>
public record LuaScript
{
    public required string Script { get; set; }
    public required Lua LuaInstance { get; set; }
    public bool OnStartCalled { get; set; }
}
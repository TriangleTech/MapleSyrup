using MapleSyrup.Core;

namespace MapleSyrup.Resources.Scripting;

/// <summary>
/// Contains the global variables that are available to Lua scripts. Any subsystems that need to be accessed from Lua should be added here.
/// <example>
/// <code>
/// Globals.EventSystem:QueueEvent("OnGameStart") -- Queue an event to be processed by the event system
/// Globals.ResourceSystem:LoadResource("BasicShader", "Shaders/BasicShader.glsl") -- Load a resource
/// Globals.NxSystem["effect"]["BasicEff.img"]["LevelUp"]["7"] -- Access a node in the NX file
/// </code>
/// </example>
/// </summary>
public record LuaGlobals
{
    public Engine Engine => Engine.Instance;
    public EventSystem EventSystem => Engine.Instance.GetSubsystem<EventSystem>();
    public ResourceSystem ResourceSystem => Engine.Instance.GetSubsystem<ResourceSystem>();
    public NxSystem NxSystem => Engine.Instance.GetSubsystem<NxSystem>();
}
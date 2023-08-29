using MapleSharp.Core;
using MapleSharp.Resources;

namespace MapleSharp.Scripting;

public class LuaFunctions
{
    public Engine Engine => Engine.Instance;
    public EventSystem EventSystem => Engine.Instance.GetSubsystem<EventSystem>();
    public ResourceSystem ResourceSystem => Engine.Instance.GetSubsystem<ResourceSystem>();
    public NxSystem NxSystem => Engine.Instance.GetSubsystem<NxSystem>();
}
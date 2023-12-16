using MapleSyrup.Gameplay.World;

namespace MapleSyrup.ECS.Components;

public class MapItem : Animated
{
    public MapItemType ItemType;
    public bool IsAnimated; // seems redundant but it's not
}
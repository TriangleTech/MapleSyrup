using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Nodes;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Gameplay.Map;

public class MapLayer : Node2D
{
    private List<MapItem> mapItems;
    
    public MapLayer(GameContext context)
        : base(context)
    {
        mapItems = new();
        Context.RegisterEventHandler(EventType.BeforeRender, SortMapItems);
    }

    public void AddItem(MapItem item)
    {
        if (item == null)
            return;
        mapItems.Add(item);
    }

    private void SortMapItems(EventData data)
    {
        // TODO: Sort
    }

    public void Draw()
    {
        var graphics = Context.GetSubsystem<GraphicsSystem>();
        
        for (int i = 0; i < mapItems.Count; i++)
        {
            graphics.DrawMapItem(mapItems[i]);
        }
    }

    public void Update(GameTime gameTime)
    {
        
    }
}
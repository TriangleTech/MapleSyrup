using MapleSyrup.Core;
using MapleSyrup.Core.Event;
using MapleSyrup.Gameplay.Map;
using MapleSyrup.Nodes;
using MapleSyrup.Resources.Nx;
using MapleSyrup.Subsystems;
using Microsoft.Xna.Framework;

namespace MapleSyrup.Gameplay;

public class MapleWorld : IGameObject
{
    private Dictionary<RenderLayer, MapLayer> mapLayers;
    private List<Node2D> monsters;
    private List<Node2D> players; 
    
    public GameContext Context { get; }
    public readonly string WorldId;
    

    public MapleWorld(GameContext context, string worldId)
    {
        Context = context;
        WorldId = worldId;
        Context.RegisterEventHandler(EventType.WorldCreated, OnWorldCreated);
        Context.RegisterEventHandler(EventType.WorldChanged, OnWorldChanged);
        Context.RegisterEventHandler(EventType.WorldDestroyed, OnWorldDestroyed);
        Context.RegisterEventHandler(EventType.RenderPass, OnRender);
        Context.RegisterEventHandler(EventType.UpdatePass, OnUpdate);
        mapLayers = new()
        {
            [RenderLayer.Background] = new (Context),
            [RenderLayer.TileObj1] = new (Context),
            [RenderLayer.TileObj2] = new (Context),
            [RenderLayer.TileObj3] = new (Context),
            [RenderLayer.TileObj4] = new (Context),
            [RenderLayer.TileObj5] = new (Context),
            [RenderLayer.TileObj6] = new (Context),
            [RenderLayer.TileObj7] = new (Context),
            [RenderLayer.Weather] = new (Context),
            [RenderLayer.Foreground] = new (Context),
        };
    }

    private void OnWorldCreated(EventData data)
    {
        var id = data[DataType.String] as string;
        var map = Context.GetSubsystem<NxSystem>().Get("Map");
        var mapNode = map["Map"][$"Map{id[0]}"][$"{id}.img"];

        MakeBack(map, mapNode);
        MakeTile(map, mapNode);
        MakeObj(map, mapNode);
    }

    private void MakeBack(NxFile map, NxNode mapNode)
    {
        var backNode = mapNode["back"];

        for (int i = 0; i < backNode.ChildCount; i++)
        {
            var backSet = backNode[$"{i}"]["bS"].To<NxStringNode>().GetString();
            var nodeId = backNode[$"{i}"]["no"].To<NxIntNode>().GetInt();
            var x = backNode[$"{i}"]["x"].To<NxIntNode>().GetInt();
            var y = backNode[$"{i}"]["y"].To<NxIntNode>().GetInt();
            var ani = backNode[$"{i}"]["ani"].To<NxIntNode>().GetInt();
            NxNode background;

            if (ani == 0)
                background = map["Back"][$"{backSet}.img"]["back"][$"{nodeId}"];
            else
                background = map["Back"][$"{backSet}.img"]["ani"][$"{nodeId}"];

            mapLayers[RenderLayer.Background].AddItem(new MapItem(Context)
            {
                Texture = Context.GetSubsystem<ResourceSystem>().Get($"Map/Back/{backSet}.img/back/{nodeId}"),
                Position = new Vector2(x, y),
                Origin = background["origin"].To<NxVectorNode>().GetVector(),
            });
        }
    }

    private void MakeTile(NxFile map, NxNode mapNode)
    {
        
    }
    
    private void MakeObj(NxFile map, NxNode mapNode)
    {
        
    }
    
    private void OnWorldChanged(EventData data)
    {
        
    }

    private void OnWorldDestroyed(EventData data)
    {
        
    }

    private void OnRender(EventData data)
    {
        var graphics = Context.GetSubsystem<GraphicsSystem>();
        graphics.BeginDraw();
        for (int i = 0; i < 10; i++)
        {
            mapLayers[(RenderLayer)i].Draw();
        }
        graphics.EndDraw();
    }

    private void OnUpdate(EventData data)
    {
        var gameTime = data[DataType.GameTime] as GameTime;
        
        for (int i = 0; i < 10; i++)
        {
            mapLayers[(RenderLayer)i].Update(gameTime);
        }
    }
}
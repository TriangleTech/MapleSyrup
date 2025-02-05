using Client.ECS.Components;
using Client.ECS.Components.Common;
using Client.ECS.Components.Map;
using Client.Resources;
using ZeroElectric.Vinculum;
using Transform = Client.ECS.Components.Common.Transform;

namespace Client.ECS.Systems.Hybrid;

public class MapObjAnimation : IUpdateSystem, IDrawSystem
{
    public void Draw(EntityFactory entityFactory, ResourceFactory resourceFactory)
    {
        var entities = entityFactory.GetAllWithComponent<MapObj>();
        foreach (var entity in entities)
        {
            var transform = entityFactory.GetComponent<Transform>(entity);
            var animation = entityFactory.GetComponent<MapObj>(entity);
            var frame = resourceFactory.GetResource<TextureResource>(animation.Textures[animation.Frame]);
            
            transform.Origin = frame.Origin;
            Raylib.DrawTextureEx(frame.Texture, transform.Position - transform.Origin, transform.Rotation, transform.Scale, Raylib.WHITE);
        }
    }

    public void Update(EntityFactory entityFactory, ResourceFactory resourceFactory, float timeDelta)
    {
        var entities = entityFactory.GetAllWithComponent<MapObj>();
        foreach (var entity in entities)
        {
            var animation = entityFactory.GetComponent<MapObj>(entity);
            if (animation.Loop)
                OnLoop(animation, entityFactory, resourceFactory, timeDelta);
            else if (animation.Blend)
                OnBlend(animation, resourceFactory, timeDelta);
            else 
                OnAnimate(animation, resourceFactory, timeDelta);
        }
    }

    private void OnAnimate(MapObj animation, ResourceFactory resourceFactory, float timeDelta)
    {
        
    }
    
    private void OnBlend(MapObj animation, ResourceFactory resourceFactory, float timeDelta)
    {
        
    }
    
    private void OnLoop(MapObj animation, EntityFactory entityFactory, ResourceFactory resourceFactory, float timeDelta)
    {
        if (animation.FrameDelay <= 0) {
            animation.Frame++;
            if (animation.Frame >= animation.FrameCount)
                animation.Frame = 0;
            
            var frame = resourceFactory.GetResource<TextureResource>(animation.Textures[animation.Frame]);
            animation.FrameDelay = frame.Delay;
        } else {
            animation.FrameDelay -= timeDelta;
        }
    }
}
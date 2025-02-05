using Client.ECS.Components;
using Client.ECS.Components.Common;
using Client.ECS.Components.Map;
using Client.Resources;
using ZeroElectric.Vinculum;
using Transform = Client.ECS.Components.Common.Transform;

namespace Client.ECS.Systems.Hybrid;

public class BackgroundAnimation : IUpdateSystem, IDrawSystem
{
    public void Draw(EntityFactory entityFactory, ResourceFactory resourceFactory)
    {
        var entities = entityFactory.GetAllWithComponent<BackgroundObj>();
        foreach (var entity in entities)
        {
            var transform = entityFactory.GetComponent<Transform>(entity);
            var animation = entityFactory.GetComponent<BackgroundObj>(entity);
            var frame = resourceFactory.GetResource<TextureResource>(animation.Textures[animation.Frame]);
            transform.Origin = frame.Origin;
            Raylib.DrawTextureEx(frame.Texture, transform.Position - transform.Origin, transform.Rotation, transform.Scale, Raylib.WHITE);
        }
    }
    
    public void Update(EntityFactory entityFactory, ResourceFactory resourceFactory, float timeDelta)
    {
        var entities = entityFactory.GetAllWithComponent<BackgroundObj>();
        foreach (var entity in entities)
        {
            var animation = entityFactory.GetComponent<BackgroundObj>(entity);
            if (animation.Animated)
                OnLoop(animation, resourceFactory, timeDelta);
        }
    }

    private void OnLoop(BackgroundObj animation, ResourceFactory resourceFactory, float timeDelta)
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
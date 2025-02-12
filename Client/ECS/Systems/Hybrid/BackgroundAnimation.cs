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
            Raylib.DrawTextureEx(frame.Texture, transform.Position - transform.Origin, transform.Rotation, transform.Scale, animation.Color);
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
            if (animation.Blend)
                OnBlend(animation, resourceFactory, timeDelta);
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

    public void OnBlend(BackgroundObj animation, ResourceFactory resourceFactory, float timeDelta)
    {
        if (animation.Frame == 0)
        {
            if (animation.FrameDelay <= 0) {
                animation.Alpha -= (int)timeDelta;
                if (animation.Alpha <= animation.Alpha0)
                {
                    animation.Frame = 1;
                    animation.Alpha = animation.Alpha0;
                    var frame = resourceFactory.GetResource<TextureResource>(animation.Textures[animation.Frame]);
                    animation.FrameDelay = frame.Delay;
                }
            } else {
                animation.FrameDelay -= timeDelta;
            }
        }
        else if (animation.Frame == 1)
        {
            if (animation.FrameDelay <= 0) {
                animation.Alpha += (int)timeDelta;
                if (animation.Alpha >= animation.Alpha1)
                {
                    animation.Frame = 0;
                    animation.Alpha = animation.Alpha1;
                    var frame = resourceFactory.GetResource<TextureResource>(animation.Textures[animation.Frame]);
                    animation.FrameDelay = frame.Delay;
                }
            } else {
                animation.FrameDelay -= timeDelta;
            }
        }

        animation.Color = new Color(255, 255, 255, animation.Alpha);
    }
}
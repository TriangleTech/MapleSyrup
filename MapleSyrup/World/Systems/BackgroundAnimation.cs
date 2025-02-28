using MapleSyrup.ECS;
using MapleSyrup.ECS.Components.Common;
using MapleSyrup.ECS.Interfaces;
using MapleSyrup.Resources;
using MapleSyrup.World.Components;
using ZeroElectric.Vinculum;

namespace MapleSyrup.World.Systems;

public class BackgroundAnimation : IUpdateSystem, IDrawSystem
{
    public void Draw(Entity entity)
    {
        if (!EntityFactory.Shared.HasComponent<BackgroundObj>(entity.Id)) return;
            var transform = EntityFactory.Shared.GetComponent<TransformComponent>(entity.Id);
            var animation = EntityFactory.Shared.GetComponent<BackgroundObj>(entity.Id);
            var frame = ResourceFactory.Shared.GetResource<TextureResource>(animation.Textures[animation.Frame]);
            transform.Origin = frame.Origin;
            Raylib.DrawTextureEx(frame.Texture, transform.Position - transform.Origin, transform.Rotation, transform.Scale, animation.Color);
        }
    
    public void Update(Entity entity, float timeDelta)
    {
        if (!EntityFactory.Shared.HasComponent<BackgroundObj>(entity.Id)) return;

            var animation = EntityFactory.Shared.GetComponent<BackgroundObj>(entity.Id);
            if (animation.Animated)
                OnLoop(animation, timeDelta);
            if (animation.Blend)
                OnBlend(animation, timeDelta);
    }

    private void OnLoop(BackgroundObj animation, float timeDelta)
    {
        if (animation.FrameDelay < 0) {
            animation.Frame++;
            if (animation.Frame >= animation.FrameCount)
                animation.Frame = 0;
            var frame = ResourceFactory.Shared.GetResource<TextureResource>(animation.Textures[animation.Frame]);
            animation.FrameDelay = frame.Delay;
        } else {
            animation.FrameDelay -= timeDelta;
        }
    }

    public void OnBlend(BackgroundObj animation, float timeDelta)
    {
        if (animation.Frame == 0)
        {
            if (animation.FrameDelay <= 0)
            {
                animation.Alpha -= (int)timeDelta;
                if (animation.Alpha <= animation.Alpha0)
                {
                    animation.Frame = 1;
                    animation.Alpha = animation.Alpha0;
                    var frame =
                        ResourceFactory.Shared.GetResource<TextureResource>(animation.Textures[animation.Frame]);
                    animation.FrameDelay = frame.Delay;
                }
            }
            else
            {
                animation.FrameDelay -= timeDelta;
            }
        }
        else if (animation.Frame == 1)
        {
            if (animation.FrameDelay <= 0)
            {
                animation.Alpha += (int)timeDelta;
                if (animation.Alpha >= animation.Alpha1)
                {
                    animation.Frame = 0;
                    animation.Alpha = animation.Alpha1;
                    var frame =
                        ResourceFactory.Shared.GetResource<TextureResource>(animation.Textures[animation.Frame]);
                    animation.FrameDelay = frame.Delay;
                }
            }
            else
            {
                animation.FrameDelay -= timeDelta;
            }
        }

        animation.Color = new Color(255, 255, 255, animation.Alpha);
    }
}
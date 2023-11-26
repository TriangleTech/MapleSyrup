namespace MapleSyrup.ECS.Systems;

public interface IUpdateSystem
{
    void Initialize();
    void Shutdown();
    void Update(float timeDelta);
}
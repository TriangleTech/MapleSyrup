namespace MapleSharp.Core.Interface;

public interface ISubsystem
{
    void Initialize();
    void Update(float timeDelta);
    void Shutdown();
}
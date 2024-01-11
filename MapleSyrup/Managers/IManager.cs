namespace MapleSyrup.Managers;

public interface IManager
{
    public void Initialize(ManagerLocator locator);
    public void Shutdown();
}
namespace MapleSyrup.Managers;

public interface IManager
{
    /// <summary>
    /// Initializes the manager and references the <see cref="ManagerLocator"/>
    /// </summary>
    /// <param name="locator"></param>
    public void Initialize(ManagerLocator locator);
    
    /// <summary>
    /// Disposes any variables within the manager.
    /// </summary>
    public void Shutdown();
}
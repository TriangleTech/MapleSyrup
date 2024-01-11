using MapleSyrup.Nx;

namespace MapleSyrup.Managers;

public class ResourceManager : IManager
{
    private ManagerLocator? _locator;
    private bool _useNx;
    private Dictionary<string, NxFile> _nxFiles;

    public ResourceManager(bool useNx)
    {
        _useNx = useNx;
        _nxFiles = new();
    }
    
    public void Initialize(ManagerLocator locator)
    {
        _locator = locator;
    }
    
    #region Get Items
    #endregion

    public void Shutdown()
    {
        
    }
}
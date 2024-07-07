using System.Runtime.CompilerServices;
using MapleSyrup.EC;
using MapleSyrup.EC.Components;
using MapleSyrup.Nx;
using MapleSyrup.Player;
using MapleSyrup.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.Managers;

public class ResourceManager : IManager
{
    private ManagerLocator? _locator;
    private Dictionary<string, NxFile> _nxFiles;
    private Dictionary<string, Texture2D> _textureCache;
    private ResourceBackend _resourceBackend;

    public ResourceManager(ResourceBackend backend)
    {
        _resourceBackend = backend;
        _nxFiles = new();
        _textureCache = new Dictionary<string, Texture2D>();
    }

    public void Initialize(ManagerLocator locator)
    {
        _locator = locator;
        _nxFiles["Map"] = new NxFile("D:/v62/Map.nx"); // Couldn't find v40 :(
        _nxFiles["Character"] = new NxFile("D:/v62/Character.nx");
        //LoadNxFiles();
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadNxFiles()
    {
        var files = Directory.GetFiles("D:/v41/", "*.nx");
        foreach (var file in files)
            _nxFiles[Path.GetFileNameWithoutExtension(file)] = new NxFile(file);
    }
    
    public string GetString(string parentFile, string resourcePath)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                return _nxFiles[parentFile].ResolvePath(resourcePath).GetString();
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }

        throw new Exception("Must set resource backend");
    }

    public int GetInt(string parentFile, string resourcePath)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                return _nxFiles[parentFile].ResolvePath(resourcePath).GetInt();
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }
        
        throw new Exception("Must set resource backend");
    }

    public double GetDouble(string parentFile, string resourcePath)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                return _nxFiles[parentFile].ResolvePath(resourcePath).GetDouble();
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }
        
        throw new Exception("Must set resource backend");
    }

    public Texture2D GetTexture(string parentFile, string resourcePath)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                if (_textureCache.TryGetValue(resourcePath, out var tex))
                    return tex;
                var texture = _nxFiles[parentFile].ResolvePath(resourcePath).GetTexture(_locator.GraphicsDevice);
                _textureCache.Add(resourcePath, texture);
                
                return _textureCache[resourcePath];
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }
        
        throw new Exception("Must set resource backend");
    }

    public Vector2 GetVector(string parentFile, string resourcePath)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                return _nxFiles[parentFile].ResolvePath(resourcePath).GetVector();
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }
        
        throw new Exception("Must set resource backend");
    }

    public int GetNodeCount(string parentFile, string resourcePath)
    {
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                var count = _nxFiles[parentFile].ResolvePath(resourcePath).ChildCount;
                _nxFiles[parentFile].ResolvePath(resourcePath).Restore();
                return count;
            case ResourceBackend.Wz:
                throw new NotImplementedException();
        }
        
        throw new Exception("Must set resource backend");
    }

    public void Shutdown()
    {
        foreach (var (_, texture) in _textureCache)
        {
            texture.Dispose();
        }

        _textureCache.Clear();
        switch (_resourceBackend)
        {
            case ResourceBackend.Nx:
                foreach (var (_, nxFile) in _nxFiles)
                {
                    nxFile.Dispose();
                }

                _nxFiles.Clear();
                break;
        }
    }
}

public enum ResourceBackend
{
    Nx,
    Wz
}
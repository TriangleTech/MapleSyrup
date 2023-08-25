namespace CM3D.Core.Windowing;

/// <summary>
/// Various window properties 
/// </summary>
public struct WindowProperties
{
    private string windowTitle;
    private int windowWidth, windowHeight;
    private bool vsync, isFullscreen;

    public WindowProperties(string title, int width, int height, bool vsyn, bool fullscreen)
    {
        windowTitle = title;
        windowWidth = width;
        windowHeight = height;
        vsync = vsyn;
        isFullscreen = fullscreen;
    }

    public string Title
    {
        get => windowTitle;
        set
        {
            if (value == string.Empty)
                return;
            windowTitle = value;
        }
    }

    public int Width
    {
        get => windowWidth;
        set => windowWidth = value;
    }

    public int Height
    {
        get => windowHeight;
        set => windowHeight = value;
    }

    public bool VSync
    {
        get => vsync;
        set => vsync = value;
    }

    public bool IsFullscreen
    {
        get => isFullscreen;
        set => isFullscreen = value;
    }
}
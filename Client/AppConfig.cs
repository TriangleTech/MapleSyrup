namespace Client;

public class AppConfig
{
    // Client Settings
    public const short VersionMajor = 62;
    public const short VersionMinor = 1;
    public const string ClientName = "MapleSyrup: Devel";
    public const int OriginalWidth = 800;
    public const int OriginalHeight = 600;
    public static int ScreenWidth { get; set; } = 800;
    public static int ScreenHeight { get; set; } = 600;
    public static bool IsFullscreen { get; set; } = false;
    public static float BaseAspectRatio => 
        (float)OriginalWidth / OriginalHeight;
    public static float CurrentAspectRatio => 
        (float)ScreenWidth / ScreenHeight;
    public static float ScaleFactor =>
        Math.Min(ScreenWidth / OriginalWidth, ScreenHeight / OriginalHeight);
    public static float ScreenOffsetX => 
        (ScreenWidth - OriginalWidth * ScaleFactor) / 2;
    public static float ScreenOffsetY => 
        (ScreenHeight - OriginalHeight * ScaleFactor) / 2;

    public static bool CloseWindow { get; set; } = false;
    public const bool IsBetaVersion = false;

    public static readonly string GameFilePath = $"D:/v{VersionMajor}";
    //public static readonly string GameFilePath = $"/home/devy/mapledev/v{VersionMajor}";
    public static readonly bool PseudoEditor = false; // This is just used to get better coordinates, since a lot of this are done by hand.
    
    // Network Settings
    public const string ServerIp = "127.0.0.1";
    public const int LoginPort = 8484;
    public static readonly int[] ChannelPorts = [7575, 7576];
}
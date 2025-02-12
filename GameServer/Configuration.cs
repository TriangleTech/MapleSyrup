namespace GameServer;

public static class Configuration
{
    // Host Settings
    public static string ServerAddress = "127.0.0.1";
    public static int LoginPort = 8484;
    public static int ChannelPort = 7575;
    public static int ChannelCount = 2;
    public static int MaxPlayers = 4;
    
    // Database Information
    public static string DatabaseName = "MapleSyrup";
    public static string DatabaseHost = "localhost";
    public static int DatabasePort = 5432; // Default PostgreSQL Port
    public static string DatabaseUser = "root";
    public static string DatabasePassword = "<PASSWORD>";
}
// See https://aka.ms/new-console-template for more information

using GameServer.Servers.Login;
var login = new LoginServer();
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    _ = login.Terminate();
};

await login.StartAsync();
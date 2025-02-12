// See https://aka.ms/new-console-template for more information

using GameServer.Servers;
using GameServer.Servers.Login;

var controller = new ServerController();
controller.Initialize();
controller.StartServers();
controller.Hold();
controller.StopServers();
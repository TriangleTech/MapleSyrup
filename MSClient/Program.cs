// See https://aka.ms/new-console-template for more information

using MapleSyrup.NX;
using MapleSyrup.Window;
using var game = new GameWindow();
game.Run();

//var file = new NxFile("D:/v62/Map.nx");
//Console.WriteLine($"Root Name: {file.Root.Name}");
//var test = file.ResolvePath("Back/grassySoil.img/back/1");//file["Back"]["grassySoil.img"]["back"]["1"];
//Console.WriteLine(test.Name);
//var img = test.GetImage();
//Console.WriteLine($"Image Width: {img.Width}");
//file.Release();
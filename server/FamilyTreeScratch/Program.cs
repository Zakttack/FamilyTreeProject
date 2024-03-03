using FamilyTreeLibrary;
string fileName = "appsettings.json";
string relativeFilePath = $@"FamilyTreeLibrary\{fileName}";
string filePath = FamilyTreeUtils.GetFilePathOf(relativeFilePath);
Console.WriteLine(filePath);
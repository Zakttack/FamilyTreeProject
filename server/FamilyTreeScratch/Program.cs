Stack<DirectoryInfo> directories = new();
string fileName = "2023PfingstenBookAlternate.pdf";
IList<string> filePaths = new List<string>();
directories.Push(new(@"C:\"));
while (directories.TryPop(out DirectoryInfo current))
{
    try
    {
        IEnumerable<FileInfo> files = current.EnumerateFiles().Where((file) =>
        {
            return file.Name == fileName;
        });
        foreach (FileInfo file in files)
        {
            filePaths.Add(file.FullName);
        }
        IEnumerable<DirectoryInfo> subDirectories = current.EnumerateDirectories();
        foreach (DirectoryInfo subDirectory in subDirectories)
        {
            directories.Push(subDirectory);
        }
    }
    catch(UnauthorizedAccessException ex)
    {
        Console.WriteLine(ex.Message);
        continue;
    }
}

foreach (string filePath in filePaths)
{
    Console.WriteLine(filePath);
}
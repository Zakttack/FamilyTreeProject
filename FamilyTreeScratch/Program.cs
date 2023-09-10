using FamilyTreeScratch;
Console.WriteLine(PdfPlayground.PdfFilePath);
Console.WriteLine(PdfPlayground.FileExists);
IEnumerable<string> lines = PdfPlayground.Content;
foreach (string line in lines)
{
    Console.WriteLine(line);
}

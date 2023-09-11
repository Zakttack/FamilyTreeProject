using FamilyTreeScratch;
IEnumerable<string> lines = PdfPlayground.Content;
foreach (string line in lines)
{
    string[] values = line.Split();
    for (int i = 0; i < values.Length; i++)
    {
        Console.WriteLine($"{i+1}: {values[i]}");
    }
    Console.WriteLine();
}

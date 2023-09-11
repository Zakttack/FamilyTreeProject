using FamilyTreeScratch;
string value = "26 June 1896";
DateTime date = Convert.ToDateTime(value);
Console.WriteLine(date.ToString().Split()[0]);
// IEnumerable<string> lines = PdfPlayground.Content;
// foreach (string line in lines)
// {
//     string[] values = line.Split();
//     for (int i = 0; i < values.Length; i++)
//     {
//         Console.WriteLine($"{i+1}: {values[i]}");
//     }
//     Console.WriteLine();
// }

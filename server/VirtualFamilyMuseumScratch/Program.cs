using System.Text.RegularExpressions;
using VirtualFamilyMuseumLibrary;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

FileInfo fileInfo = new("C:\\GitHubProjects\\FamilyTreeProject\\Resources\\template_samples\\test-template-1.pdf");
using PdfReader reader = new(fileInfo);
using PdfDocument document = new(reader);
PdfPage page1 = document.GetPage(1);
string pageText = PdfTextExtractor.GetTextFromPage(page1);
Regex hierarchicalCoordinateRegex = new(@"^\d(\.\d)*\)", RegexOptions.Compiled);
IList<string> initialLines = [.. pageText.Split('\n')];
int index = 0;
while (index < initialLines.Count)
{
    if (hierarchicalCoordinateRegex.Matches(initialLines[index]).Count > 0)
    {
        index++;
    }
    else
    {
        string previousLine = initialLines[index - 1];
        string currentLine = initialLines[index];
        initialLines[index - 1] = $"{previousLine.Trim()} {currentLine.Trim()}";
        initialLines.RemoveAt(index);
    }
}
foreach (string line in initialLines)
{
    Console.WriteLine(line);
}

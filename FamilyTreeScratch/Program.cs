using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF;

Queue<string> textLines = PdfUtils.GetPDFLinesAsQueue(31, @"C:\Users\zakme\Documents\FamilyTreeProject\Resources\PfingstenBook2023.pdf");
IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> nodesWithoutChildren = PdfUtils.ParseAsFamilyNodes(textLines);
foreach (KeyValuePair<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> node in nodesWithoutChildren)
{
    foreach (KeyValuePair<int,Family> familyPairs in node.Value)
    {
        Console.WriteLine(familyPairs.Value);
        Console.WriteLine();
    }
}
// PdfClient client = new(@"C:\Users\zakme\Documents\FamilyTreeProject\Resources\PfingstenBook2023.pdf", 26);
// client.LoadNodes();
// IEnumerable<Family> families = client.Nodes;
// foreach (Family family in families)
// {
//     Console.WriteLine(family);
// }
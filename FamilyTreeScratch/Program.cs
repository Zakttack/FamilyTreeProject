using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF;

Queue<string> textLines = PdfUtils.GetPDFLinesAsQueue(26, @"C:\Users\zakme\Documents\FamilyTreeProject\Resources\PfingstenBook2023.pdf");
IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> nodesWithoutChildren = PdfUtils.ParseAsFamilyNodes(textLines);
foreach (KeyValuePair<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> node in nodesWithoutChildren)
{
    foreach (KeyValuePair<int,Family> familyPairs in node.Value)
    {
        Console.WriteLine(familyPairs.Value);
        Console.WriteLine();
    }
}
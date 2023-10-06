using FamilyTreeLibrary;
using FamilyTreeLibrary.OrderingType;
PdfClient client = new("PfingstenBook2023.pdf", 110);
// IReadOnlyDictionary<AbstractOrderingType[],string> nodes = client.Nodes;
// foreach (KeyValuePair<AbstractOrderingType[], string> node in nodes)
// {
//     PrintArray(node.Key);
//     Console.WriteLine(node.Value);
//     Console.WriteLine();
// }

// static void PrintArray(AbstractOrderingType[] collection)
// {
//     string output = "[";
//     for (int i = 0; i < collection.Length - 1; i++)
//     {
//         output += $"{collection[i].ConversionPair.Value},";
//     }
//     output += $"{collection[^1].ConversionPair.Value}]:";
//     Console.WriteLine(output);
// }

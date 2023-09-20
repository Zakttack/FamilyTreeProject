using FamilyTreeLibrary;
using FamilyTreeLibrary.OrderingType;
IReadOnlyDictionary<AbstractOrderingType[],string> nodes = FamilyTreeUtils.ExpressAsNodes("PfingstenBook2023.pdf");
foreach (KeyValuePair<AbstractOrderingType[], string> node in nodes)
{
    PrintArray(node.Key);
    Console.WriteLine(node.Value);
    Console.WriteLine();
}

static void PrintArray(AbstractOrderingType[] collection)
{
    string output = "[";
    for (int i = 0; i < collection.Length - 1; i++)
    {
        output += $"{collection[i].ConversionPair.Value},";
    }
    output += $"{collection[^1].ConversionPair.Value}]:";
    Console.WriteLine(output);
}

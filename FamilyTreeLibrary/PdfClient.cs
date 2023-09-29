using FamilyTreeLibrary.OrderingType;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace FamilyTreeLibrary
{
    public class PdfClient
    {
        private readonly SortedDictionary<AbstractOrderingType[],string> nodes;

        public PdfClient(string pdfFileName)
        {
            FilePath = GetFileNameFromResources(Directory.GetCurrentDirectory(), pdfFileName);
            nodes = new(new OrderingTypeComparer());
            IList<string> tokens = GetPDFTokens();
            LoadNodes(new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)}, FamilyTreeUtils.SubTokenCollection(tokens, tokens.IndexOf(AbstractOrderingType.GetOrderingType(1,1).ConversionPair.Value) + 1, tokens.Count - 1));
        }

        public string FilePath
        {
            get;
        }

        public IReadOnlyDictionary<AbstractOrderingType[],string> Nodes
        {
            get
            {
                return nodes;
            }
        }

        private PdfDocument Document
        {
            get
            {
                return new(new PdfReader(FilePath));
            }
        }

        private void AddNode(AbstractOrderingType[] orderingTypes, IList<string> contents)
        {
            if (!nodes.ContainsKey(orderingTypes))
            {
                nodes.Add(orderingTypes, string.Join(' ', contents));
            }
        }

        private string GetFileNameFromResources(string currentPath, string fileNameWithExtension)
        {
            string[] parts = currentPath.Split('\\');
            string current = parts[^1];
            if (current != "FamilyTreeProject")
            {
                return GetFileNameFromResources(currentPath[..(currentPath.Length - current.Length - 1)], fileNameWithExtension);
            }
            return $"{currentPath}\\Resources\\{fileNameWithExtension}";
        }

        private IList<string> GetPDFTokens()
        {
            IList<string> tokens = new List<string>();
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            for (int n = 1; n <= Document.GetNumberOfPages(); n++)
            {
                string page = PdfTextExtractor.GetTextFromPage(Document.GetPage(n), strategy);
                string[] parts = page.Split(' ', '\n');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] != "")
                    {
                        tokens.Add(parts[i]);
                    }
                }
            }
            return tokens;
        }

        private void LoadNodes(AbstractOrderingType[] orderingTypes, IList<string> tokens)
        {
            IList<string> contents;
            int end1 = orderingTypes.Length < 6 ? tokens.IndexOf(AbstractOrderingType.GetOrderingType(1, orderingTypes.Length + 1).ConversionPair.Value) - 1 : -2;
            int end2 = tokens.IndexOf(AbstractOrderingType.GetOrderingType(orderingTypes[^1].ConversionPair.Key + 1, orderingTypes.Length).ConversionPair.Value) - 1;
            if (end1 <= 0 && end2 <= 0)
            {
                AddNode(orderingTypes, tokens);
            }
            else if (end1 <= 0 && end2 > 0)
            {
                contents = FamilyTreeUtils.SubTokenCollection(tokens, 0, end2);
                AddNode(orderingTypes, contents);
                LoadNodes(FamilyTreeUtils.ReplaceWithIncrementByKey(orderingTypes), FamilyTreeUtils.SubTokenCollection(tokens, end2 + 2, tokens.Count - 1));
            }
            else if (end1 > 0 && end2 < end1)
            {
                contents = FamilyTreeUtils.SubTokenCollection(tokens, 0, end1);
                AddNode(orderingTypes, contents);
                LoadNodes(FamilyTreeUtils.IncrementGeneration(orderingTypes), FamilyTreeUtils.SubTokenCollection(tokens, end1 + 2, tokens.Count - 1));
            }
            else if (end1 < end2)
            {
                contents = FamilyTreeUtils.SubTokenCollection(tokens, 0, end1);
                AddNode(orderingTypes, contents);
                LoadNodes(FamilyTreeUtils.IncrementGeneration(orderingTypes), FamilyTreeUtils.SubTokenCollection(tokens, end1 + 2, end2));
                LoadNodes(FamilyTreeUtils.ReplaceWithIncrementByKey(orderingTypes), FamilyTreeUtils.SubTokenCollection(tokens, end2 + 2, tokens.Count - 1));
            }
        }

        private class OrderingTypeComparer : IComparer<AbstractOrderingType[]>
        {
            public int Compare(AbstractOrderingType[] a, AbstractOrderingType[] b)
            {
                for (int i = 0; i < a.Length && i < b.Length; i++)
                {
                    if (a[i].CompareTo(b[i]) < 0)
                    {
                        return -1;
                    }
                    else if (a[i].CompareTo(b[i]) > 0)
                    {
                        return 1;
                    }
                }
                return a.Length - b.Length;
            }
        }
    }
}
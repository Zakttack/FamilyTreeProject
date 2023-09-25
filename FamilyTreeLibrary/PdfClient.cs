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
            Task nodesTask = Task.Run(async () =>
            {
                IList<string> tokens = await GetPDFTokens();
                IReadOnlyList<int> positions = await GetOrderingTypePositions(tokens);
                for (int p = 0; p < positions.Count; p++)
                {
                    IReadOnlySet<AbstractOrderingType> orderingTypes = await GetAbstractOrderingTypes(tokens, positions, p);
                    nodes.Add(orderingTypes.ToArray(), GetContent(tokens, positions, p));
                }
            });
            nodesTask.Start();
            nodesTask.Wait();
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

        private async Task<AbstractOrderingType> FindOrderingType(IList<string> tokens, IReadOnlyList<int> positions, int position)
        {
            AbstractOrderingType orderingType = null;
            Task<AbstractOrderingType> findOrderingTypeTask = Task.Run(() =>
            {
                if (position <= 0)
                {
                    AbstractOrderingType.TryGetOrderingType(out orderingType, 1, 1);
                }
                else
                {
                    for (int generation = 1; generation <= 6; generation++)
                    {
                        if (AbstractOrderingType.TryGetOrderingType(out orderingType, tokens[positions[position]], generation) 
                            && (generation > 1 || (generation == 1 && orderingType.ConversionPair.Key > 1)))
                        {
                            break;
                        }
                    }
                }
                return orderingType;
            });
            return await findOrderingTypeTask;
        }

        private async Task<IReadOnlySet<AbstractOrderingType>> GetAbstractOrderingTypes(IList<string> tokens, IReadOnlyList<int> positions, int position)
        {
            SortedSet<AbstractOrderingType> orderingTypes = new();
            Task<SortedSet<AbstractOrderingType>> getOrderingTypesTask = Task.Run(async () =>
            {
                AbstractOrderingType orderingType = await FindOrderingType(tokens, positions, position);
                if (orderingType != null)
                {
                    orderingTypes.Add(orderingType);
                    if (AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType firstOrderingType, orderingType.ConversionPair.Key, 1) &&
                        orderingType.CompareTo(firstOrderingType) != 0)
                    {
                        for (int p = position - 1; p >= 0; p--)
                        {
                            orderingTypes.Add(await FindOrderingType(tokens, positions, p));
                        }
                    }
                }
                return orderingTypes;
            });
            return await getOrderingTypesTask;
        }

        private string GetContent(IList<string> tokens, IReadOnlyList<int> positions, int position)
        {
            return string.Join(' ', position < positions.Count - 1 ? FamilyTreeUtils.GetSubCollection(tokens.ToArray(), positions[position] + 1, positions[position + 1] - 1) : FamilyTreeUtils.GetSubCollection(tokens.ToArray(), positions[position] + 1));
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

        private async Task<IReadOnlyList<int>> GetOrderingTypePositions(IList<string> tokens)
        {
            ISet<int> positions = new SortedSet<int>();
            Task<List<int>> positionsTask = Task.Run(async () =>
            {
                for (int generation = 1; generation <= 6; generation++)
                {
                    positions.UnionWith(await GetOrderingTypePositions(tokens, generation));
                }
                return positions.ToList();
            });
            return await positionsTask;
        }

        private async Task<IReadOnlySet<int>> GetOrderingTypePositions(IList<string> tokens, int generation)
        {
            SortedSet<int> positions = new();
            Task<SortedSet<int>> positionsOfGenerationTask = Task.Run(() =>
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, tokens[i], generation))
                    {
                        positions.Add(i);
                    }
                }
                return positions;
            });
            return await positionsOfGenerationTask;
        }

        private async Task<IList<string>> GetPDFTokens()
        {
            IList<string> tokens = new List<string>();
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            Task<IList<string>> tokensTask = Task.Run(() =>
            {
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
            });
            return await tokensTask;
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
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF.Models;

namespace FamilyTreeLibrary.PDF
{
    public class PdfClient
    {
        private readonly ICollection<Family> nodes;

        private readonly List<Section> familyNodeCollection;

        public PdfClient(string pdfFileName, int lineLimit = int.MaxValue)
        {
            FilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), pdfFileName);
            nodes = new SortedSet<Family>();
            Root = new Section(Array.Empty<AbstractOrderingType>(), FamilyTreeUtils.Root);
            familyNodeCollection = new();
            LineLimit = lineLimit;
        }

        public string FilePath
        {
            get;
        }

        public IEnumerable<Family> Nodes
        {
            get
            {
                return nodes;
            }
        }

        public async Task LoadNodes()
        {
            Console.WriteLine($"Reading {FilePath}.");
            AbstractOrderingType[] currentOrderingType = Array.Empty<AbstractOrderingType>();
            Queue<AbstractOrderingType> previousPossibilities = new();
            string previousLine = "";
            int iterationNumber = 1;
            IEnumerable<IEnumerable<string>> pages = await PdfUtils.GetLinesFromDocument(FilePath);
            await Task.Run(async () => {
                foreach (IEnumerable<string> page in pages)
                {
                    foreach (string line in page)
                    {
                        try
                        {
                            string currentLine = line.Trim();
                            Queue<AbstractOrderingType> currentPossibilities = FamilyTreeUtils.GetOrderingTypeByLine(currentLine);
                            if (await PdfUtils.IsInLaw(currentPossibilities, previousLine, currentLine))
                            {
                                previousLine += $"  {currentLine}";
                            }
                            else if (await PdfUtils.IsMember(currentPossibilities))
                            {
                                if (previousLine != "" && previousPossibilities.Count > 0)
                                {
                                    string[] tokens = await PdfUtils.ReformatLine(previousLine);
                                    Queue<Line> lines = await PdfUtils.GetLines(tokens);
                                    Family node = await PdfUtils.GetFamily(lines);
                                    AbstractOrderingType[] temp = currentOrderingType;
                                    currentOrderingType = await PdfUtils.FillSection(familyNodeCollection, temp, previousPossibilities, node);
                                    if (iterationNumber == familyNodeCollection.Count - 1)
                                    {
                                        iterationNumber++;
                                    }
                                    Console.WriteLine($"Section #{iterationNumber}: {node}");
                                    if (familyNodeCollection.Count >= LineLimit)
                                    {
                                        return;
                                    }
                                }
                                else if (previousLine != "")
                                {
                                    throw new InvalidOperationException("The ordering type is un-defined.");
                                }
                                previousLine = currentLine;
                                previousPossibilities = currentPossibilities;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex.GetType().Name} on Section #{++iterationNumber}: {ex.Message}\n{ex.StackTrace}");
                        }
                    }
                }
            });
        }

        public async Task AttachNodes()
        {
            await Task.Run(async () =>
            {
                await AttachNodes(familyNodeCollection, Root);
            });
        }

        private async Task AttachNodes(IReadOnlyList<Section> familyNodeCollection, Section root)
        {
            ICollection<Section> subFamilyNodeCollection = new List<Section>();
            await Task.Run(async () =>
            {
                foreach (Section familyNode in familyNodeCollection)
                {
                    if (familyNode.OrderingType.Length == root.OrderingType.Length + 1)
                    {
                        root.Node.Children.Add(familyNode.Node);
                        familyNode.Node.Parent = root.Node;
                        nodes.Add(familyNode.Node);
                        await AttachNodes(subFamilyNodeCollection.ToList(), familyNode);
                        subFamilyNodeCollection.Clear();
                    }
                    else
                    {
                        subFamilyNodeCollection.Add(familyNode);
                    }
                }
            });
        }

        private Section Root
        {
            get;
        }

        private int LineLimit
        {
            get;
        }
    }
}
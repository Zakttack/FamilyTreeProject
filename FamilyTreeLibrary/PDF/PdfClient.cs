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

        public void LoadNodes()
        {
            Console.WriteLine($"Reading {FilePath}.");
            AbstractOrderingType[] currentOrderingType = Array.Empty<AbstractOrderingType>();
            Queue<AbstractOrderingType> previousPossibilities = new();
            string previousLine = "";
            int iterationNumber = 1;
            IReadOnlyCollection<string> pdfLines = PdfUtils.GetLinesFromDocument(FilePath);
            Console.WriteLine($"{pdfLines.Count} lines were detected.");
            foreach (string line in pdfLines)
            {
                try
                {
                    string currentLine = line.Trim();
                    Queue<AbstractOrderingType> currentPossibilities = FamilyTreeUtils.GetOrderingTypeByLine(currentLine, pdfLines.Count);
                    if (PdfUtils.IsInLaw(currentPossibilities, previousLine, currentLine))
                    {
                        previousLine += $"  {currentLine}";
                    }
                    else if (PdfUtils.IsMember(currentPossibilities))
                    {
                        if (previousLine != "" && previousPossibilities.Count > 0)
                        {
                            string[] tokens = PdfUtils.ReformatLine(previousLine);
                            Queue<Line> lines = PdfUtils.GetLines(tokens);
                            Family node = PdfUtils.GetFamily(lines);
                            AbstractOrderingType[] temp = currentOrderingType;
                            currentOrderingType = PdfUtils.FillSection(familyNodeCollection, temp, previousPossibilities, node);
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
                            throw new InvalidOperationException("The ordering type is undefined.");
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
            Console.WriteLine($"{familyNodeCollection.Count} sections were detected.");
        }

        public void AttachNodes()
        {
            AttachNodes(familyNodeCollection, Root);
        }

        private void AttachNodes(IReadOnlyList<Section> familyNodeCollection, Section root)
        {
            ICollection<Section> subFamilyNodeCollection = new List<Section>();
            foreach (Section familyNode in familyNodeCollection)
            {
                if (familyNode.OrderingType.Length == root.OrderingType.Length + 1)
                {
                    root.Node.Children.Add(familyNode.Node);
                    familyNode.Node.Parent = root.Node;
                    nodes.Add(familyNode.Node);
                    AttachNodes(subFamilyNodeCollection.ToList(), familyNode);
                    subFamilyNodeCollection.Clear();
                }
                else
                {
                    subFamilyNodeCollection.Add(familyNode);
                }
            }
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
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF.Models;

namespace FamilyTreeLibrary.PDF
{
    public class PdfClient
    {
        private readonly ICollection<Family> nodes;

        public PdfClient(string pdfFileName)
        {
            FilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), pdfFileName);
            nodes = new SortedSet<Family>();
            Root = new Section(Array.Empty<AbstractOrderingType>(), FamilyTreeUtils.Root);
            FamilyNodeCollection = new();
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
            int sectionNumber = 1;
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
                        CreateNode(previousLine, previousPossibilities, ref currentOrderingType, ref sectionNumber);
                        previousLine = currentLine;
                        previousPossibilities = currentPossibilities;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.GetType().Name} on Section #{++sectionNumber}: {ex.Message}\n{ex.StackTrace}");
                }
            }
            CreateNode(previousLine, previousPossibilities, ref currentOrderingType, ref sectionNumber);
            Console.WriteLine($"{FamilyNodeCollection.Count} sections were detected.");
        }

        public void AttachNodes()
        {
            AttachNodes(FamilyNodeCollection, Root);
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

        private void CreateNode(string previousLine, Queue<AbstractOrderingType> previousPossibilities, ref AbstractOrderingType[] currentOrderingType, ref int sectionNumber)
        {
            if (previousLine != "" && previousPossibilities.Count > 0)
            {
                string[] tokens = PdfUtils.ReformatLine(previousLine);
                Queue<Line> lines = PdfUtils.GetLines(tokens);
                Family node = PdfUtils.GetFamily(lines);
                AbstractOrderingType[] temp = currentOrderingType;
                currentOrderingType = PdfUtils.AddSection(FamilyNodeCollection, temp, previousPossibilities, node, ref sectionNumber);
            }
            else if (previousLine != "")
            {
                throw new InvalidOperationException("The ordering type is undefined.");
            }
        }

        private List<Section> FamilyNodeCollection
        {
            get;
        }

        private Section Root
        {
            get;
        }
    }
}
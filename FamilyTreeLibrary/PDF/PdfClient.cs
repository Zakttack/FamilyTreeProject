using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF.Models;
using Serilog;

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
            FamilyTreeUtils.InitializeLogger();
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
            Log.Debug($"Reading {FilePath}.");
            AbstractOrderingType[] currentOrderingType = Array.Empty<AbstractOrderingType>();
            Queue<AbstractOrderingType> previousPossibilities = new();
            string previousLine = "";
            int sectionNumber = 1;
            IReadOnlyCollection<string> pdfLines = PdfUtils.GetLinesFromDocument(FilePath);
            Log.Debug($"{pdfLines.Count} lines were detected.");
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
                    Log.Fatal($"{ex.GetType().Name} on Section #{++sectionNumber}: {ex.Message}\n{ex.StackTrace}");
                }
            }
            CreateNode(previousLine, previousPossibilities, ref currentOrderingType, ref sectionNumber);
            Log.Debug($"{FamilyNodeCollection.Count} sections were detected.");
        }

        public void AttachNodes()
        {
            try
            {
                Log.Debug("Nodes are connecting.");
                AttachNodes(FamilyNodeCollection, Root);
                Log.Debug("Nodes are connected.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void AttachNodes(IReadOnlyList<Section> familyNodeCollection, Section root)
        {
            ICollection<Section> subFamilyNodeCollection = new List<Section>();
            Section previousFamilyNode = root;
            foreach (Section familyNode in familyNodeCollection)
            {
                if (familyNode.OrderingType.Length == root.OrderingType.Length + 1)
                {
                    root.Node.Children.Add(familyNode.Node);
                    Log.Debug($"Parent of current: {root}");
                    familyNode.Node.Parent = root.Node;
                    Log.Debug($"Current: {familyNode}");
                    nodes.Add(familyNode.Node);
                    Section tempFamilyNode = previousFamilyNode;
                    AttachSubNodes(ref subFamilyNodeCollection, tempFamilyNode);
                    previousFamilyNode = familyNode;
                }
                else
                {
                    subFamilyNodeCollection.Add(familyNode);
                }
            }
            AttachSubNodes(ref subFamilyNodeCollection, previousFamilyNode);
        }

        private void AttachSubNodes(ref ICollection<Section> subFamilyNodeCollection, Section root)
        {
            if (subFamilyNodeCollection.Count > 0)
            {
                AttachNodes(subFamilyNodeCollection.ToList(), root);
                subFamilyNodeCollection.Clear();
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
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
            string previousLine = "";
            int iterationNumber = 1;
            IEnumerable<IEnumerable<string>> pages = PdfUtils.GetLinesFromDocument(FilePath);
            foreach (IEnumerable<string> page in pages)
            {
                foreach (string line in page)
                {
                    try
                    {
                        string currentLine = line.Trim();
                        Queue<AbstractOrderingType> orderingTypePossibilities = FamilyTreeUtils.GetOrderingTypeByLine(currentLine);
                        if (PdfUtils.IsInLaw(orderingTypePossibilities, previousLine, currentLine))
                        {
                            previousLine += $"  {currentLine}";
                        }
                        else if (PdfUtils.IsMember(orderingTypePossibilities))
                        {
                            if (previousLine != "")
                            {
                                string[] tokens = PdfUtils.ReformtLine(previousLine);
                                Queue<Line> lines = PdfUtils.GetLines(tokens);
                                Family node = PdfUtils.GetFamily(lines);
                                AbstractOrderingType[] temp = currentOrderingType;
                                currentOrderingType = PdfUtils.FillSection(familyNodeCollection, temp, orderingTypePossibilities, node);
                                Console.WriteLine($"Section #{iterationNumber}: {node}");
                                if (familyNodeCollection.Count >= LineLimit)
                                {
                                    return;
                                }
                            }
                            previousLine = currentLine;
                        }
                    }
                    catch (Exception ex)
                    {
                       Console.WriteLine($"{ex.GetType().Name} on Section #{iterationNumber}: {ex.Message}\n{ex.StackTrace}");
                    }
                    finally
                    {
                        if (familyNodeCollection.Count > 0)
                        {
                            iterationNumber++;
                        }
                    }
                }
            }
            Console.WriteLine("Nodes have been loaded.");
        }

        public void AttachNodes()
        {
            Console.WriteLine("Now we need to attach the nodes.");
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
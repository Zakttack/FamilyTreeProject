using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.OrderingType.Comparers;
using FamilyTreeLibrary.PDF.Models;

namespace FamilyTreeLibrary.PDF
{
    public class PdfClient
    {
        private readonly Queue<string> textLines;
        private readonly SortedDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> nodes;

        public PdfClient(string pdfFileName, int lineLimit = int.MaxValue)
        {
            FilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), pdfFileName);
            nodes = new(Comparer);
            textLines = PdfUtils.GetPDFLinesAsQueue(lineLimit, pdfFileName);
        }

        public string FilePath
        {
            get;
        }

        public IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> Nodes
        {
            get
            {
                return nodes;
            }
        }

        public void LoadNodes()
        {
            AbstractOrderingType[] previous = new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)};
            AbstractOrderingType[] current = new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)};
            int i = 0;
            while (textLines.Count > 0)
            {
                string line = textLines.Dequeue();
                AbstractOrderingType orderingType = FamilyTreeUtils.GetOrderingTypeByLine(line);
                string[] tokens = line.Split(' ');
                Queue<Line> lines = PdfUtils.GetLines(tokens);
                IReadOnlyDictionary<AbstractOrderingType,Family> subNodes = PdfUtils.ParseAsSubNodes(orderingType, lines);
                foreach (KeyValuePair<AbstractOrderingType,Family> subNode in subNodes)
                {
                    if (subNode.Key == default && Comparer.Compare(previous, current) < 0)
                    {
                        Family first = nodes[previous].Peek().Value;
                        subNode.Value.Member.BirthDate = first.Member.BirthDate;
                        subNode.Value.Member.DeceasedDate = first.Member.DeceasedDate;
                        nodes[previous].Enqueue(new(i, subNode.Value));
                    }
                    else if (subNode.Key != default && subNode.Key.ConversionPair.Key == 1)
                    {
                        previous = current;
                        current = FamilyTreeUtils.IncrementGeneration(previous);
                        Queue<KeyValuePair<int,Family>> families = new();
                        families.Enqueue(new(i, subNode.Value));
                        nodes.Add(previous, families);
                    }
                    else if (subNode.Key != default && subNode.Key.ConversionPair.Key > 1)
                    {
                        previous = current;
                        current = FamilyTreeUtils.ReplaceWithIncrementByKey(previous);
                        Queue<KeyValuePair<int,Family>> families = new();
                        families.Enqueue(new(i, subNode.Value));
                        nodes.Add(previous, families);
                    }
                    i++;
                }
            }
        }

        private IComparer<AbstractOrderingType[]> Comparer
        {
            get
            {
                return new OrderingTypeComparer();
            }
        }
    }
}
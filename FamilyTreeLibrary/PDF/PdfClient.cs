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
            AbstractOrderingType[] current = new AbstractOrderingType[0];
            int i = 0;
            while (textLines.Count > 0)
            {
                string line = textLines.Dequeue();
                AbstractOrderingType orderingType = FamilyTreeUtils.GetOrderingTypeByLine(line);
                current = FamilyTreeUtils.NextOrderingType(current, orderingType);
                string[] tokens = line.Split(' ');
                Queue<Line> lines = PdfUtils.GetLines(tokens);
                IReadOnlyDictionary<AbstractOrderingType,Family> subNodes = PdfUtils.ParseAsSubNodes(orderingType, lines);
                foreach (KeyValuePair<AbstractOrderingType,Family> subNode in subNodes)
                {
                    if (subNode.Key == default)
                    {
                        AbstractOrderingType[] previous = FamilyTreeUtils.PreviousOrderingType(current);
                        Family first = nodes[previous].Peek().Value;
                        subNode.Value.Member.BirthDate = first.Member.BirthDate;
                        subNode.Value.Member.DeceasedDate = first.Member.DeceasedDate;
                        nodes[previous].Enqueue(new(i, subNode.Value));
                    }
                    else
                    {
                        Queue<KeyValuePair<int,Family>> families = new();
                        families.Enqueue(new(i, subNode.Value));
                        nodes.Add(FamilyTreeUtils.CopyOrderingType(current), families);
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
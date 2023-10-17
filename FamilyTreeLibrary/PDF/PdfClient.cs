using FamilyTreeLibrary.Comparers;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;

namespace FamilyTreeLibrary.PDF
{
    public class PdfClient
    {
        private readonly ICollection<Family> nodes;
        private readonly IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> familyNodeCollection;

        public PdfClient(string pdfFileName, int lineLimit = int.MaxValue)
        {
            FilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), pdfFileName);
            nodes = new SortedSet<Family>(new FamilyComparer());
            familyNodeCollection = PdfUtils.ParseAsFamilyNodes(PdfUtils.GetPDFLinesAsQueue(lineLimit, pdfFileName));
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
            List<Queue<KeyValuePair<int,Family>>> subFamilyNodeCollection = new();
            foreach (KeyValuePair<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> familyNodes in familyNodeCollection)
            {
                if (familyNodes.Key.Length == 1)
                {
                    Queue<KeyValuePair<int,Family>> familyQueue = new();
                    while(familyNodes.Value.Count > 0)
                    {
                        KeyValuePair<int,Family> familyPair = familyNodes.Value.Dequeue();
                        nodes.Add(familyPair.Value);
                        FamilyTreeUtils.Root.Children.Add(familyPair.Value.Member);
                        familyQueue.Enqueue(familyPair);
                    }
                    subFamilyNodeCollection.Add(familyQueue);
                }
            }
            LoadNodes(subFamilyNodeCollection, 2, 0);
        }

        private void LoadNodes(IReadOnlyList<Queue<KeyValuePair<int,Family>>> subFamilyNodeCollection, int generation, int index)
        {
            if (subFamilyNodeCollection.Count > 0)
            {
                List<Queue<KeyValuePair<int,Family>>> subCollection = new();
                foreach (KeyValuePair<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> familyNodes in familyNodeCollection)
                {
                    if (familyNodes.Key.Length == generation)
                    {
                        KeyValuePair<int,Family> start;
                        if (index < subFamilyNodeCollection.Count - 1)
                        {
                            start = subFamilyNodeCollection[index].Dequeue();
                            KeyValuePair<int,Family> end;
                            if (subFamilyNodeCollection[index].Count > 0)
                            {
                                end = subFamilyNodeCollection[index].Peek();
                            }
                            else
                            {
                                end = subFamilyNodeCollection[index + 1].Peek();
                            }
                            Queue<KeyValuePair<int,Family>> familyQueue = new();
                            while (familyNodes.Value.TryPeek(out KeyValuePair<int,Family> current) && start.Key < current.Key && current.Key < end.Key)
                            {
                                KeyValuePair<int,Family> familarPair = familyNodes.Value.Dequeue();
                                nodes.Add(familarPair.Value);
                                start.Value.Children.Add(familarPair.Value.Member);
                                familyQueue.Enqueue(familarPair);
                            }
                            if (familyQueue.Count > 0)
                            {
                                subCollection.Add(familyQueue);
                            }
                        }
                        else if (index == subFamilyNodeCollection.Count - 1)
                        {
                            start = subFamilyNodeCollection[index].Dequeue();
                            Queue<KeyValuePair<int,Family>> familyQueue = new();
                            while (familyNodes.Value.TryPeek(out KeyValuePair<int,Family> current) && start.Key < current.Key)
                            {
                                KeyValuePair<int,Family> familarPair = familyNodes.Value.Dequeue();
                                nodes.Add(familarPair.Value);
                                start.Value.Children.Add(familarPair.Value.Member);
                                familyQueue.Enqueue(familarPair);
                            }
                            if (familyQueue.Count > 0)
                            {
                                subCollection.Add(familyQueue);
                            }
                        }
                        else
                        {
                            LoadNodes(subCollection, generation + 1, 0);
                        }
                    }
                }
                LoadNodes(subCollection, generation, index + 1);
            }
        }
    }
}
using FamilyTreeLibrary.OrderingType;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary
{
    public class PdfClient
    {
        private readonly SortedDictionary<AbstractOrderingType[],ICollection<Family>> nodes;

        public PdfClient(string pdfFileName, int lineLimit = int.MaxValue)
        {
            FilePath = GetFileNameFromResources(Directory.GetCurrentDirectory(), pdfFileName);
            nodes = new(new OrderingTypeComparer());
            Queue<string> lines = GetPDFLines(110);
            //LoadNodes(new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)}, FamilyTreeUtils.SubTokenCollection(tokens, tokens.IndexOf(AbstractOrderingType.GetOrderingType(1,1).ConversionPair.Value) + 1, tokens.Count - 1));
        }

        public string FilePath
        {
            get;
        }

        public IReadOnlyDictionary<AbstractOrderingType[],ICollection<Family>> Nodes
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

        private Queue<string> GetPDFLines(int lineLimit)
        {
            Queue<string> lines = new();
            for (int n = 1; n <= Document.GetNumberOfPages(); n++)
            {
                string page = PdfTextExtractor.GetTextFromPage(Document.GetPage(n));
                string[] parts = page.Split("\n");
                string temp2 = "";
                foreach (string line in parts)
                {
                    if (lines.Count < lineLimit)
                    {
                        string temp = line.TrimStart();
                        AbstractOrderingType orderingType = FamilyTreeUtils.GetOrderingTypeByLine(temp);
                        if (orderingType == null && lines.Count > 0 && Regex.IsMatch(temp, "^[a-zA-Z0-9 ]*$"))
                        {
                            temp2 += " " + temp;
                        }
                        else if (orderingType != null)
                        {
                            if (temp2 != "")
                            {
                                string item = temp2;
                                lines.Enqueue(item);
                            }
                            temp2 = temp;
                        }
                    }
                }
            }
            return lines;
        }

        private void LoadNodes(Queue<string> lines)
        {

        }

        private IReadOnlyDictionary<AbstractOrderingType,Family> ParseFamilyPerOrderingType(Queue<string> lines)
        {
            string line = lines.Dequeue();
            AbstractOrderingType orderingType = FamilyTreeUtils.GetOrderingTypeByLine(line);
            
            return null;
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
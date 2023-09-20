using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
namespace FamilyTreeLibrary
{
    public class FamilyTreeUtils
    {
        public static DateTime DefaultDate
        {
            get
            {
                return new();
            }
        }

        public static IComparer<DateTime> DateComp
        {
            get
            {
                return new DateComparer();
            }
        }

        public static IComparer<Family> FamilyComparer
        {
            get
            {
                return new PersonComparer();
            }
        }

        public static IReadOnlyDictionary<AbstractOrderingType[],string> ExpressAsNodes(string pdfFileName)
        {
            SortedDictionary<AbstractOrderingType[],string> nodes = new(new OrderingTypeComparer());
            IList<string> tokens = GetPDFContents(pdfFileName);
            IReadOnlyList<int> positions = GetOrderingTypePositions(tokens);
            for (int p = 0; p < positions.Count; p++)
            {
                nodes.Add(GetAbstractOrderingTypes(tokens, positions, p).ToArray(), GetContent(tokens, positions, p));
            }
            return nodes;
        }

        private static AbstractOrderingType FindOrderingType(IList<string> tokens, IReadOnlyList<int> positions, int position)
        {
            if (position <= 0)
            {
                return AbstractOrderingType.GetOrderingType(tokens[positions[0]], 1);
            }
            int generation = 1;
            while (generation <= 6)
            {
                try
                {
                    AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(tokens[positions[position]], generation);
                    if (generation == 1 && orderingType.ConversionPair.Key == 1)
                    {
                        generation++;
                    }
                    else
                    {
                        return orderingType;
                    }
                }
                catch (ArgumentException)
                {
                    generation++;
                }
            }
            throw new ArgumentException("Ordering Type Not Found.");
        }

        private static IReadOnlySet<AbstractOrderingType> GetAbstractOrderingTypes(IList<string> tokens, IReadOnlyList<int> positions, int position)
        {
            AbstractOrderingType orderingType = FindOrderingType(tokens, positions, position);
            SortedSet<AbstractOrderingType> orderingTypes = new()
            {
                orderingType
            };
            if (orderingType.CompareTo(AbstractOrderingType.GetOrderingType(orderingType.ConversionPair.Key, 1)) != 0)
            {
                for (int p = position - 1; p >= 0; p--)
                {
                    orderingTypes.Add(FindOrderingType(tokens, positions, p));
                }
            }
            return orderingTypes;
        }

        private static string GetContent(IList<string> tokens, IReadOnlyList<int> positions, int position)
        {
            return string.Join(' ', position < positions.Count - 1 ? GetSubCollection(tokens.ToArray(), positions[position] + 1, positions[position + 1] - 1) : GetSubCollection(tokens.ToArray(), positions[position] + 1));
        }

        private static string GetFileNameFromResources(string currentPath, string fileNameWithExtension)
        {
            string[] parts = currentPath.Split('\\');
            string current = parts[^1];
            if (current != "FamilyTreeProject")
            {
                return GetFileNameFromResources(currentPath[..(currentPath.Length - current.Length - 1)], fileNameWithExtension);
            }
            return $"{currentPath}\\Resources\\{fileNameWithExtension}";
        }

        private static IReadOnlyList<int> GetOrderingTypePositions(IList<string> tokens)
        {
            ISet<int> positions = new SortedSet<int>();
            for (int generation = 1; generation <= 6; generation++)
            {
                positions.UnionWith(GetOrderingTypePositions(tokens, generation));
            }
            return positions.ToList();
        }

        private static IReadOnlySet<int> GetOrderingTypePositions(IList<string> tokens, int generation)
        {
            SortedSet<int> positions = new();
            for (int i = 0; i < tokens.Count; i++)
            {
                try
                {
                    AbstractOrderingType.GetOrderingType(tokens[i], generation);
                    positions.Add(i);
                }
                catch (ArgumentException)
                {
                }
            }
            return positions;
        }

        private static IList<string> GetPDFContents(string pdfFileName)
        {
            PdfReader reader = new(GetFileNameFromResources(Directory.GetCurrentDirectory(), pdfFileName));
            PdfDocument document = new (reader);
            IList<string> lines = new List<string>();
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            string page = PdfTextExtractor.GetTextFromPage(document.GetPage(1), strategy);
            string[] data = page.Split('\n');
            foreach (string d in data)
            {
                lines.Add(d);
            }
            return lines;
        }

        private static string[] GetSubCollection(string[] collection, int start, int end)
        {
            return collection.AsSpan(start, end - start + 1).ToArray();
        }

        private static string[] GetSubCollection(string[] collection, int start)
        {
            return collection.AsSpan(start).ToArray();
        }

        private class DateComparer : IComparer<DateTime>
        {
            public int Compare(DateTime a, DateTime b)
            {
                int yearDiff = a.Year - b.Year;
                int monthDiff = a.Month - b.Month;
                int dayDiff = a.Day - b.Day;
                if (yearDiff == 0)
                {
                    if (monthDiff == 0)
                    {
                        return dayDiff;
                    }
                    return monthDiff;
                }
                return yearDiff;
            }
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

        private class PersonComparer : IComparer<Family>
        {
            public int Compare(Family a, Family b)
            {
                return DateComp.Compare(a.Couple[0].BirthDate, b.Couple[0].BirthDate);
            }
        }
    }
}

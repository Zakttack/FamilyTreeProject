using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF.Models;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using FamilyTreeLibrary.OrderingType.Comparers;

namespace FamilyTreeLibrary.PDF
{
    public static class PdfUtils
    {
        public static bool IsInLaw(Queue<AbstractOrderingType> orderingTypePossibilities, string previousLine, string currentLine)
        {
            string inLawPattern = @"^[a-zA-Z0-9’,\-. ]*$";
            return orderingTypePossibilities.Count == 0 && previousLine != "" && Regex.IsMatch(currentLine, inLawPattern);
        }

        public static bool IsMember(Queue<AbstractOrderingType> orderingTypePossibilities)
        {
            return orderingTypePossibilities.Count > 0; 
        }
        
        public static AbstractOrderingType[] FillSection(ICollection<Section> sections, AbstractOrderingType[] previous, Queue<AbstractOrderingType> possibilities, Family node)
        {
            Section section;
            while (possibilities.Count > 0)
            {
                AbstractOrderingType type = possibilities.Dequeue();
                if (IsDuplicate(sections, previous, type, node))
                {
                    AbstractOrderingType[] orderingType = previous;
                    while (!orderingType[^1].Equals(type))
                    {
                        orderingType = FamilyTreeUtils.PreviousOrderingType(orderingType);
                    }
                    section = new(orderingType, node);
                    sections.Add(section);
                    break;
                }
                AbstractOrderingType[] next = FamilyTreeUtils.NextOrderingType(previous, type);
                if (next != null)
                {
                    section = new(next, node);
                    sections.Add(section);
                    return next;
                }
            }
            return previous;
        }

        public static Family GetFamily(Queue<Line> lines)
        {
            Line memberLine;
            Person member;
            Line inLawLine;
            Person inLaw;
            Family fam;
            if (lines.Count % 2 == 0)
            {
                memberLine = lines.Dequeue();
                member = new(memberLine.Name);
                if (memberLine.Dates.TryDequeue(out FamilyTreeDate memberBirthDate))
                {
                    member.BirthDate = memberBirthDate;
                }
                memberLine.Dates.TryDequeue(out FamilyTreeDate marriage);
                if (memberLine.Dates.TryDequeue(out FamilyTreeDate memberDeceasedDate))
                {
                    member.DeceasedDate = memberDeceasedDate;
                }
                inLawLine = lines.Dequeue();
                inLaw = new(inLawLine.Name);
                if (inLawLine.Dates.TryDequeue(out FamilyTreeDate inLawBirthDate))
                {
                    inLaw.BirthDate = inLawBirthDate;
                }
                if (inLawLine.Dates.TryDequeue(out FamilyTreeDate inLawDeceasedDate))
                {
                    inLaw.DeceasedDate = inLawDeceasedDate;
                }
                fam = new Family(member)
                {
                    InLaw = inLaw
                };
                if (!new FamilyTreeDate(0).Equals(marriage))
                {
                    fam.MarriageDate = marriage;
                }
            }
            else
            {
                memberLine = lines.Dequeue();
                member = new(memberLine.Name);
                if (memberLine.Dates.TryDequeue(out FamilyTreeDate memberBirth))
                {
                    member.BirthDate = memberBirth;
                }
                if (memberLine.Dates.TryDequeue(out FamilyTreeDate memberDeceased))
                {
                    member.DeceasedDate = memberDeceased;
                }
                fam = new Family(member);
            }
            return fam;
        }

        public static Queue<Line> GetLines(string[] tokens)
        {
            Queue<Line> lines = new();
            string tempName = "";
            string tempDate = "";
            bool readAsName = true;
            Line tempLine = new();
            for (int i = 1; i < tokens.Length - 1; i++)
            {
                if (tokens[i] == "" && tempName != "")
                {
                    if (tempLine.Name != null)
                    {
                        lines.Enqueue(tempLine.Copy());
                        tempLine = new();
                    }
                    tempLine.Name = tempName.Trim();
                    tempName = "";
                }
                else if (tokens[i] == "" && tempDate != "")
                {
                    FamilyTreeDate dateItem2 = new(tempDate.Trim());
                    tempLine.Dates.Enqueue(dateItem2);
                    tempDate = "";
                }
                else if (readAsName)
                {
                    tempName += $"{tokens[i]} ";
                }
                else
                {
                    tempDate += $"{tokens[i]} ";
                }
                readAsName = !Regex.IsMatch(tokens[i + 1], FamilyTreeUtils.NUMBER_PATTERN) && !Regex.IsMatch(tokens[i+1], FamilyTreeUtils.RANGE_PATTERN) && !new FamilyTreeDate(0).Months.ContainsKey(tokens[i+1]);
            }
            if (Regex.IsMatch(tokens[^1], FamilyTreeUtils.NUMBER_PATTERN) | Regex.IsMatch(tokens[^1], FamilyTreeUtils.RANGE_PATTERN))
            {
                FamilyTreeDate dateItem3 = new(tempDate + tokens[^1]);
                tempLine.Dates.Enqueue(dateItem3);
            }
            else
            {
                if (lines.Count == 0 && tempLine.Name != null)
                {
                    lines.Enqueue(tempLine.Copy());
                }
                tempLine = new(tokens.Length > 1 ? $"{tempName}{tokens[^1]}" : "");
            }
            lines.Enqueue(tempLine);
            return lines;
        }

        public static IReadOnlyCollection<string> GetLinesFromDocument(string fileName)
        {
            PdfReader reader = new(fileName);
            PdfDocument document = new(reader);
            IReadOnlyCollection<string> pdfLines = new List<string>();
            string spacePattern = "^ +$";
            bool spaceFilter(string value) => !Regex.IsMatch(value, spacePattern);
            for (int pageNumber = 1; pageNumber <= document.GetNumberOfPages(); pageNumber++)
            {
                IList<string> pageLines = PdfTextExtractor.GetTextFromPage(document.GetPage(pageNumber)).Split('\n').Where(spaceFilter).ToList();
                pageLines.RemoveAt(0);
                pageLines.RemoveAt(pageLines.Count - 1);
                IEnumerable<string> initial = pdfLines;
                pdfLines = initial.Concat(pageLines).ToList();
            }
            return pdfLines;
        }

        public static string[] ReformatLine(string line)
        {
            ICollection<string> adjustedTokens = new List<string>();
            int spaceCount = 0;
            IEnumerable<string> tokens = line.Split(' ');
            foreach (string token in tokens)
            {
                if (token == "")
                {
                    spaceCount++;
                }
                else
                {
                    spaceCount = 0;
                }
                if (spaceCount < 2)
                {
                    adjustedTokens.Add(token);
                }
            }
            return adjustedTokens.ToArray();
        }

        private static bool IsDuplicate(ICollection<Section> sections, AbstractOrderingType[] previous, AbstractOrderingType temp, Family node)
        {
            ICollection<AbstractOrderingType> current = new SortedSet<AbstractOrderingType>();
            foreach (AbstractOrderingType type in previous)
            {
                current.Add(type);
                if (temp.Equals(type))
                {
                    break;
                }
            }
            if (current.Count != previous.Length)
            {
                IEqualityComparer<AbstractOrderingType[]> duplicateChecker = new OrderingTypeComparer();
                Section[] sectionMatches = sections.Where((sec) => duplicateChecker.Equals(sec.OrderingType, current.ToArray())).ToArray();
                return sectionMatches.Length != 0 && FamilyTreeUtils.MemberEquivalent(sectionMatches[0].Node, node);
            }
            return false;
        }
    }
}
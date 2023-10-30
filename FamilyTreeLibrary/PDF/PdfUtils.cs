using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF.Models;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace FamilyTreeLibrary.PDF
{
    public static class PdfUtils
    {
        public static bool IsInLaw(Queue<AbstractOrderingType> orderingTypePossibilities, string previousLine, string currentLine)
        {
            string inLawPattern = "^[a-zA-Z0-9,. ]*$";
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
            FamilyTreeDate marriage;
            Line inLawLine;
            Person inLaw;
            if (lines.Count % 2 == 0)
            {
                memberLine = lines.Dequeue();
                member = new(memberLine.Name)
                {
                    BirthDate = memberLine.Dates.Dequeue()
                };
                marriage = memberLine.Dates.Dequeue();
                if (memberLine.Dates.TryDequeue(out FamilyTreeDate value2))
                {
                    member.DeceasedDate = value2;
                }
                inLawLine = lines.Dequeue();
                inLaw = new(inLawLine.Name)
                {
                    BirthDate = inLawLine.Dates.Dequeue()
                };
                if (inLawLine.Dates.TryDequeue(out FamilyTreeDate value3))
                {
                    inLaw.DeceasedDate = value3;
                }
                return new(member)
                {
                    InLaw = inLaw,
                    MarriageDate = marriage
                };
            }
            memberLine = lines.Dequeue();
            member = new(memberLine.Name);
            if (memberLine.Dates.TryDequeue(out FamilyTreeDate value))
            {
                member.BirthDate = value;
            }
            if (memberLine.Dates.TryDequeue(out FamilyTreeDate value1))
            {
                member.DeceasedDate = value1;
            }
            return new(member);
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
                readAsName = !Regex.IsMatch(tokens[i + 1], FamilyTreeUtils.NUMBER_PATTERN) && !Regex.IsMatch(tokens[i+1], FamilyTreeUtils.RANGE_PATTERN) && !FamilyTreeDate.Months.Contains(tokens[i+1]);
            }
            if (Regex.IsMatch(tokens[^1], FamilyTreeUtils.NUMBER_PATTERN) | Regex.IsMatch(tokens[^1], FamilyTreeUtils.RANGE_PATTERN))
            {
                FamilyTreeDate dateItem3 = new(tempDate + tokens[^1]);
                tempLine.Dates.Enqueue(dateItem3);
            }
            else
            {
                tempLine.Name = tokens.Length > 1 ? $"{tempName}{tokens[^1]}" : "";
            }
            lines.Enqueue(tempLine);
            return lines;
        }

        public static IEnumerable<IEnumerable<string>> GetLinesFromDocument(string fileName)
        {
            PdfReader reader = new(fileName);
            PdfDocument document = new(reader);
            ICollection<IEnumerable<string>> pages = new List<IEnumerable<string>>();
            string spacePattern = "^ +$";
            bool spaceFilter(string value) => !Regex.IsMatch(value, spacePattern);
            for (int pageNumber = 1; pageNumber <= document.GetNumberOfPages(); pageNumber++)
            {
                IList<string> page = PdfTextExtractor.GetTextFromPage(document.GetPage(pageNumber)).Split('\n').Where(spaceFilter).ToList();
                page.RemoveAt(0);
                page.RemoveAt(page.Count - 1);
                pages.Add(page);
            }
            return pages;
        }

        public static string[] ReformtLine(string line)
        {
            IEnumerable<string> tokens = line.Split(' ');
            ICollection<string> adjustedTokens = new List<string>();
            bool encounteredSpace = false;
            foreach (string token in tokens)
            {
                if (!encounteredSpace)
                {
                    adjustedTokens.Add(token);
                }
                encounteredSpace = token == "";
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
                foreach (Section sec in sections)
                {
                    if (sec.OrderingType == current.ToArray())
                    {
                        return FamilyTreeUtils.MemberEquivalent(sec.Node, node);
                    }
                }
            }
            return false;
        }
    }
}
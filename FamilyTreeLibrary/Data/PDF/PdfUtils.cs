using FamilyTreeLibrary.Data.PDF.Models;
using FamilyTreeLibrary.Data.PDF.OrderingType;
using FamilyTreeLibrary.Data.PDF.OrderingType.Comparers;
using FamilyTreeLibrary.Models;
using Serilog;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace FamilyTreeLibrary.Data.PDF
{
    public static partial class PdfUtils
    {
        public static AbstractOrderingType[] AddSection(ICollection<Section> sections, AbstractOrderingType[] previous, Queue<AbstractOrderingType> possibilities, Family node, ref int sectionNumber)
        {
            Section section;
            while (possibilities.Count > 0)
            {
                AbstractOrderingType type = possibilities.Dequeue();
                if (HasAddionalPartner(sections, previous, type, node))
                {
                    AbstractOrderingType[] orderingType = previous;
                    while (!orderingType[^1].Equals(type))
                    {
                        orderingType = AbstractOrderingType.PreviousOrderingType(orderingType);
                    }
                    section = new(orderingType, node);
                    sections.Add(section);
                    if (sectionNumber == sections.Count - 1)
                    {
                        sectionNumber++;
                    }
                    Log.Debug($"Section #{sectionNumber}: {section}");
                    break;
                }
                AbstractOrderingType[] next = AbstractOrderingType.NextOrderingType(previous, type);
                if (next != null)
                {
                    section = new(next, node);
                    sections.Add(section);
                    if (sectionNumber == sections.Count - 1)
                    {
                        sectionNumber++;
                    }
                    Log.Debug($"Section #{sectionNumber}: {section}");
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
                member = new(memberLine.Name)
                {
                    BirthDate = memberLine.Dates.TryDequeue(out FamilyTreeDate memberBirthDate) ? memberBirthDate : FamilyTreeDate.DefaultDate
                };
                FamilyTreeDate marriageDate = memberLine.Dates.TryDequeue(out FamilyTreeDate marriage) ? marriage : FamilyTreeDate.DefaultDate;
                member.DeceasedDate = memberLine.Dates.TryDequeue(out FamilyTreeDate memberDeceasedDate) ? memberDeceasedDate : FamilyTreeDate.DefaultDate;
                inLawLine = lines.Dequeue();
                inLaw = new(inLawLine.Name)
                {
                    BirthDate = inLawLine.Dates.TryDequeue(out FamilyTreeDate inLawBirthDate) ? inLawBirthDate : FamilyTreeDate.DefaultDate,
                    DeceasedDate = inLawLine.Dates.TryDequeue(out FamilyTreeDate inLawDeceasedDate) ? inLawDeceasedDate : FamilyTreeDate.DefaultDate
                };
                fam = new Family(member)
                {
                    InLaw = inLaw,
                    MarriageDate = marriageDate
                };
            }
            else
            {
                memberLine = lines.Dequeue();
                member = new(memberLine.Name)
                {
                    BirthDate = memberLine.Dates.TryDequeue(out FamilyTreeDate memberBirth) ? memberBirth : FamilyTreeDate.DefaultDate,
                    DeceasedDate = memberLine.Dates.TryDequeue(out FamilyTreeDate memberDeceased) ? memberDeceased : FamilyTreeDate.DefaultDate
                };
                fam = new Family(member)
                {
                    InLaw = null,
                    MarriageDate = FamilyTreeDate.DefaultDate
                };
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
                readAsName = !FamilyTreeUtils.NumberPattern().IsMatch(tokens[i + 1]) && !FamilyTreeUtils.RangePattern().IsMatch(tokens[i+1]) && !FamilyTreeDate.DefaultDate.Months.ContainsKey(tokens[i+1]);
            }
            if (FamilyTreeUtils.NumberPattern().IsMatch(tokens[^1]) || FamilyTreeUtils.RangePattern().IsMatch(tokens[^1]))
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

        public static IReadOnlyCollection<string> GetLinesFromDocument(string filePath)
        {
            PdfReader reader = new(filePath);
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

        public static string GetPersonLogName(Section section)
        {
            return section.Node.Member.Name is null ? "Root" : section.Node.Member.Name;
        }

        public static bool IsInLaw(Queue<AbstractOrderingType> orderingTypePossibilities, string previousLine, string currentLine)
        {
            string inLawPattern = @"^[a-zA-Z0-9',\-. ]*$";
            return orderingTypePossibilities.Count == 0 && previousLine != "" && Regex.IsMatch(currentLine, inLawPattern);
        }

        public static bool IsMember(Queue<AbstractOrderingType> orderingTypePossibilities)
        {
            return orderingTypePossibilities.Count > 0; 
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

        private static bool HasAddionalPartner(ICollection<Section> sections, AbstractOrderingType[] previous, AbstractOrderingType temp, Family node)
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
            IEqualityComparer<AbstractOrderingType[]> duplicateChecker = new OrderingTypeComparer();
            Section[] sectionMatches = sections.Where((sec) => duplicateChecker.Equals(sec.OrderingType, current.ToArray())).ToArray();
            return sectionMatches.Length != 0 && sectionMatches[0].OrderingType[^1].Equals(temp) && sectionMatches[0].Node.Member.BirthDate.Equals(node.Member.BirthDate);
        }
    }
}
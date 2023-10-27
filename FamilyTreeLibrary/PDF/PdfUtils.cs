using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.OrderingType.Comparers;
using FamilyTreeLibrary.PDF.Models;
using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace FamilyTreeLibrary.PDF
{
    public static class PdfUtils
    {
        public static AbstractOrderingType[] FillSection(ICollection<Section> sections, AbstractOrderingType[] previous, string line, int index)
        {
            Queue<AbstractOrderingType> possibilities = FamilyTreeUtils.GetOrderingTypeByLine(line);
            string[] tokens = line.Split(' ');
            Queue<Line> lines = GetLines(tokens);
            Family node = GetFamily(lines);
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
                    section = new(orderingType, node, index);
                    sections.Add(section);
                    break;
                }
                AbstractOrderingType[] next = FamilyTreeUtils.NextOrderingType(previous, type);
                if (next != null)
                {
                    section = new(next, node, index);
                    sections.Add(section);
                    return next;
                }
            }
            return previous;
        }
        public static Queue<string> GetPDFLinesAsQueue(int lineLimit, string fileName)
        {
            Queue<string> lines = new();
            string previousLine = "";
            IEnumerable<IEnumerable<string>> pages = GetLinesFromDocument(fileName);
            foreach (IEnumerable<string> page in pages)
            {
                foreach (string line in page)
                {
                    string currentLine = line.TrimStart();
                    Queue<AbstractOrderingType> orderingTypePossibilities = FamilyTreeUtils.GetOrderingTypeByLine(currentLine);
                    if (IsInLaw(orderingTypePossibilities, previousLine, currentLine))
                    {
                        previousLine += $" {currentLine}";
                    }
                    else if (IsMember(orderingTypePossibilities))
                    {
                        if (previousLine != "")
                        {
                            string[] tokens = previousLine.Split(' ');
                            StringBuilder tokenRebuilder = new();
                            for (int i = 0; i < tokens.Length; i++)
                            {
                                if (tokens[i] == "June")
                                {
                                    tokenRebuilder.Append(Regex.IsMatch(tokens[i+1], FamilyTreeUtils.NUMBER_PATTERN) ? " Jun" : $" {tokens[i]}");
                                }
                                else if (i > 0)
                                {
                                    string tokenUpdate = FamilyTreeUtils.ReformatToken(tokens[i]);
                                    if (tokenUpdate.Length > 0)
                                    {
                                        tokenRebuilder.Append($" {tokenUpdate}");
                                    }
                                }
                                else
                                {
                                    tokenRebuilder.Append($"{tokens[i]}");
                                }
                            }
                            lines.Enqueue(tokenRebuilder.ToString());
                            if (lines.Count >= lineLimit)
                            {
                                return lines;
                            }
                        }
                        previousLine = currentLine;
                    }
                }
            }
            return lines;
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
                if (tokens[i] != "")
                {
                    if (readAsName)
                    {
                        if (tempLine.Name != null)
                        {
                            lines.Enqueue(tempLine.Copy());
                            tempLine = new();
                        }
                        tempName += $"{tokens[i]} ";
                    }
                    else
                    {
                        if (tempName.Length > 0)
                        {
                            tempLine.Name = tempName.Trim();
                        }
                        tempName = "";
                        tempDate += $"{tokens[i]} ";
                        if (tokens[i].Length == 4 && tokens[i] != "June")
                        {
                            FamilyTreeDate dateItem2 = new(tempDate.Trim());
                            tempLine.Dates.Enqueue(dateItem2);
                            tempDate = "";
                        }
                    }
                    readAsName = !Regex.IsMatch(tokens[i + 1], FamilyTreeUtils.NUMBER_PATTERN) && !Regex.IsMatch(tokens[i+1], FamilyTreeUtils.RANGE_PATTERN) && !FamilyTreeDate.Months.Contains(tokens[i+1]);
                }
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

        public static ICollection<Section> GetSections(Queue<string> textLines)
        {
            ICollection<Section> sections = new SortedSet<Section>();
            int index = 0;
            AbstractOrderingType[] previous = Array.Empty<AbstractOrderingType>();
            while (textLines.Count > 0)
            {
                AbstractOrderingType[] temp = previous;
                previous = FillSection(sections, temp, textLines.Dequeue(), index++);
            }
            return sections;
        }

        public static bool IsDuplicate(ICollection<Section> sections, AbstractOrderingType[] previous, AbstractOrderingType temp, Family node)
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

        public static IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> ParseAsFamilyNodes(Queue<string> textLines)
        {
            SortedDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> familyNodes = new(new OrderingTypeComparer());
            AbstractOrderingType[] current = Array.Empty<AbstractOrderingType>();
            int i = 0;
            while (textLines.Count > 0)
            {
                string line = textLines.Dequeue();
                Queue<AbstractOrderingType> orderingTypePossibilities = FamilyTreeUtils.GetOrderingTypeByLine(line);
                AbstractOrderingType orderingType = null;
                AbstractOrderingType[] temp;
                while (orderingTypePossibilities.Count > 0)
                {
                    orderingType = orderingTypePossibilities.Dequeue();
                    temp = FamilyTreeUtils.NextOrderingType(current, orderingType);
                    if (temp != null)
                    {
                        current = temp;
                        break;
                    } 
                }
                string[] tokens = line.Split(' ');
                Queue<Line> lines = GetLines(tokens);
                IReadOnlyDictionary<AbstractOrderingType,Family> subNodes = ParseAsSubNodes(orderingType, lines);
                foreach (KeyValuePair<AbstractOrderingType,Family> subNode in subNodes)
                {
                    if (subNode.Key == default)
                    {
                        AbstractOrderingType[] previous = current;
                        do
                        {
                            previous = FamilyTreeUtils.PreviousOrderingType(previous);
                        } while(previous.Length > 0 && !FamilyTreeUtils.MemberEquivalent(familyNodes[previous].Peek().Value, subNode.Value));
                        if (previous.Length > 0)
                        {
                            Family first = familyNodes[previous].Peek().Value;
                            subNode.Value.Member.BirthDate = first.Member.BirthDate;
                            subNode.Value.Member.DeceasedDate = first.Member.DeceasedDate;
                            familyNodes[previous].Enqueue(new(i, subNode.Value));
                        }
                    }
                    else
                    {
                        Queue<KeyValuePair<int,Family>> families = new();
                        families.Enqueue(new(i, subNode.Value));
                        familyNodes.Add(FamilyTreeUtils.CopyOrderingType(current), families);
                    }
                    i++;
                }
            }
            return familyNodes;
        }

        public static IReadOnlyDictionary<AbstractOrderingType,Family> ParseAsSubNodes(AbstractOrderingType orderingType, Queue<Line> lines)
        {
            Dictionary<AbstractOrderingType,Family> subNodes = new();
            Family family = GetFamily(lines);
            if (family != default)
            {
                subNodes.Add(orderingType, family);
            }
            Family duplicateFamily = GetDuplicateFamily(lines);
            if (duplicateFamily != default)
            {
                subNodes.Add(default, duplicateFamily);
            }
            return subNodes;
        }

        private static Family GetDuplicateFamily(Queue<Line> lines)
        {
            if (lines.Count == 2)
            {
                Line memberLine = lines.Dequeue();
                Line inLawLine = lines.Dequeue();
                Person member = new(memberLine.Name);
                FamilyTreeDate marriage;
                if (memberLine.Dates.Count > 1)
                {
                    member.BirthDate = memberLine.Dates.Dequeue();
                    marriage = memberLine.Dates.Dequeue();
                    if (memberLine.Dates.TryDequeue(out FamilyTreeDate value))
                    {
                        member.DeceasedDate = value;
                    }
                }
                else
                {
                    marriage = memberLine.Dates.Dequeue();
                }
                Person inLaw = new(inLawLine.Name)
                {
                    BirthDate = inLawLine.Dates.Dequeue()
                };
                if (inLawLine.Dates.TryDequeue(out FamilyTreeDate value1))
                {
                    inLaw.DeceasedDate = value1;
                }
                return new(member)
                {
                    InLaw = inLaw,
                    MarriageDate = marriage
                };
            }
            return default;
        }

        private static Family GetFamily(Queue<Line> lines)
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

        private static IEnumerable<IEnumerable<string>> GetLinesFromDocument(string fileName)
        {
            PdfReader reader = new(fileName);
            PdfDocument document = new(reader);
            ICollection<IEnumerable<string>> pages = new List<IEnumerable<string>>();
            for (int pageNumber = 1; pageNumber <= document.GetNumberOfPages(); pageNumber++)
            {
                IList<string> page = PdfTextExtractor.GetTextFromPage(document.GetPage(pageNumber)).Split('\n').ToList();
                page.RemoveAt(0);
                page.RemoveAt(page.Count - 1);
                pages.Add(page);
            }
            return pages;
        }

        private static bool IsInLaw(Queue<AbstractOrderingType> orderingTypePossibilities, string previousLine, string currentLine)
        {
            string inLawPattern = "^[a-zA-Z0-9,. ]*$";
            return orderingTypePossibilities.Count == 0 && previousLine != "" && Regex.IsMatch(currentLine, inLawPattern);
        }

        private static bool IsMember(Queue<AbstractOrderingType> orderingTypePossibilities)
        {
            return orderingTypePossibilities.Count > 0;
        }
    }
}
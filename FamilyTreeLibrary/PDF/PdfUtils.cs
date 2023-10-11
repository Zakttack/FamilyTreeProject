using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF.Models;
using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace FamilyTreeLibrary.PDF
{
    public static class PdfUtils
    {

        public static Queue<string> GetPDFLinesAsQueue(int lineLimit, string fileName)
        {
            Queue<string> lines = new();
            PdfReader reader = new(fileName);
            PdfDocument document = new(reader);
            for (int n = 1; n <= document.GetNumberOfPages(); n++)
            {
                string page = PdfTextExtractor.GetTextFromPage(document.GetPage(n));
                string[] parts = page.Split("\n");
                string temp2 = "";
                foreach (string line in parts)
                {
                    string temp = line.TrimStart();
                    AbstractOrderingType orderingType = FamilyTreeUtils.GetOrderingTypeByLine(temp);
                    if (orderingType == null && temp2 != "" && Regex.IsMatch(temp, "^[a-zA-Z0-9 ]*$"))
                    {
                        temp2 += $" {temp}";
                    }
                    else if (orderingType != null)
                    {
                        if (temp2 != "")
                        {
                            string[] tokens = temp2.Split(' ');
                            StringBuilder tokenRebuilder = new();
                            foreach (string token in tokens)
                            {
                                tokenRebuilder.Append($"{FamilyTreeUtils.ReformatToken(token)} ");
                            }
                            lines.Enqueue(tokenRebuilder.ToString().Trim());
                            if (lines.Count >= lineLimit)
                            {
                                return lines;
                            }
                        }
                        temp2 = temp;
                    }
                }
            }
            return lines;
        }

        public static Queue<Line> GetLines(string[] tokens)
        {
            Queue<Line> lines = new();
            string pattern = "^\\d+$";
            string tempName = "";
            string tempDate = "";
            bool readAsName = true;
            Line tempLine = new();
            for (int i = 1; i < tokens.Length - 1; i++)
            {
                if (readAsName)
                {
                    if (tempDate.Length > 0)
                    {
                        DateTime dateItem1 = Convert.ToDateTime(tempDate.Trim());
                        tempLine.Dates.Enqueue(dateItem1);
                    }
                    tempDate = "";
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
                    if (FamilyTreeUtils.IsMonth(tokens[i - 1]))
                    {
                        DateTime dateItem2 = Convert.ToDateTime(tempDate.Trim());
                        tempLine.Dates.Enqueue(dateItem2);
                        tempDate = "";
                    }
                }
                readAsName = !Regex.IsMatch(tokens[i + 1], pattern) && !FamilyTreeUtils.IsMonth(tokens[i+1]);
            }
            DateTime dateItem3 = Convert.ToDateTime(tempDate + tokens[^1]);
            tempLine.Dates.Enqueue(dateItem3);
            lines.Enqueue(tempLine);
            return lines;
        }

        public static IReadOnlyDictionary<AbstractOrderingType,Family> ParseAsSubNodes(AbstractOrderingType orderingType, Queue<Line> lines)
        {
            Dictionary<AbstractOrderingType,Family> subNodes = new();
            DateTime marriage;
            Family fam;
            Family famDuplicate;
            Line memberLine;
            Line inLawLine;
            Line duplicateMemberLine;
            Line duplicateInLawLine;
            Person member;
            Person inLaw;
            Person duplicateMember;
            Person duplicateInlaw;
            switch (lines.Count)
            {
                case 1: memberLine = lines.Dequeue();
                member = memberLine.Dates.Count > 1 ? new(memberLine.Name, memberLine.Dates.Dequeue(), memberLine.Dates.Dequeue()) : new(memberLine.Name, memberLine.Dates.Dequeue());
                fam = new(member);
                subNodes.Add(orderingType, fam);
                break;
                case 2: memberLine = lines.Dequeue();
                member = new(memberLine.Name, memberLine.Dates.Dequeue());
                marriage = memberLine.Dates.Dequeue();
                if (memberLine.Dates.Count > 0)
                {
                    member.DeceasedDate = memberLine.Dates.Dequeue();
                }
                inLawLine = lines.Dequeue();
                inLaw = inLawLine.Dates.Count > 1 ? new(inLawLine.Name, inLawLine.Dates.Dequeue(), inLawLine.Dates.Dequeue()) : new(inLawLine.Name, inLawLine.Dates.Dequeue());
                fam = new(member, inLaw, marriage);
                subNodes.Add(orderingType, fam);
                break;
                case 3: memberLine = lines.Dequeue();
                member = memberLine.Dates.Count > 1 ? new(memberLine.Name, memberLine.Dates.Dequeue(), memberLine.Dates.Dequeue()) : new(memberLine.Name, memberLine.Dates.Dequeue());
                fam = new(member);
                subNodes.Add(orderingType, fam);
                duplicateMemberLine = lines.Dequeue();
                duplicateMember = new(duplicateMemberLine.Name, default);
                duplicateInLawLine = lines.Dequeue();
                duplicateInlaw = duplicateInLawLine.Dates.Count > 1 ? new(duplicateInLawLine.Name, duplicateInLawLine.Dates.Dequeue(), duplicateInLawLine.Dates.Dequeue()) : new(duplicateInLawLine.Name, duplicateInLawLine.Dates.Dequeue());
                famDuplicate = new(duplicateMember, duplicateInlaw, duplicateMemberLine.Dates.Dequeue());
                subNodes.Add(default, famDuplicate);
                break;
                case 4: memberLine = lines.Dequeue();
                member = new(memberLine.Name, memberLine.Dates.Dequeue());
                marriage = memberLine.Dates.Dequeue();
                if (memberLine.Dates.Count > 0)
                {
                    member.DeceasedDate = memberLine.Dates.Dequeue();
                }
                inLawLine = lines.Dequeue();
                inLaw = inLawLine.Dates.Count > 1 ? new(inLawLine.Name, inLawLine.Dates.Dequeue(), inLawLine.Dates.Dequeue()) : new(inLawLine.Name, inLawLine.Dates.Dequeue());
                fam = new(member, inLaw, marriage);
                subNodes.Add(orderingType, fam);
                duplicateMemberLine = lines.Dequeue();
                duplicateMember = new(duplicateMemberLine.Name, default);
                duplicateInLawLine = lines.Dequeue();
                duplicateInlaw = duplicateInLawLine.Dates.Count > 1 ? new(duplicateInLawLine.Name, duplicateInLawLine.Dates.Dequeue(), duplicateInLawLine.Dates.Dequeue()) : new(duplicateInLawLine.Name, duplicateInLawLine.Dates.Dequeue());
                famDuplicate = new(duplicateMember, duplicateInlaw, duplicateMemberLine.Dates.Dequeue());
                subNodes.Add(default, famDuplicate);
                break;
            }
            return subNodes;
        }
    }
}
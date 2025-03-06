using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace FamilyTreeLibrary.Data.Files
{
    public partial class TemplateGenerator
    {
        private const string DEFAULT_FILE_PATH = "../../../../resources/PfingstenFamilyAlternative.txt";
        private const string DEFAULT_WRITE_FILE_PATH = "../../../../resources/2023PfingstenBookAlternate.pdf";
        private readonly string? filePath;
        private readonly IExtendedLogger logger;
        private readonly FamilyTreeConfiguration configuration;
        private readonly FamilyTreeVault vault;
        private readonly IReadOnlyDictionary<HierarchialCoordinate, FamilyTreeNode> nodeCollection;
        private readonly ISet<Person> people;
        private readonly ISet<Partnership> partnerships;
        private readonly string inheritedFamilyName;

        public TemplateGenerator(FamilyTreeConfiguration configuration, FamilyTreeVault vault, IExtendedLogger logger, string? filePath = DEFAULT_FILE_PATH)
        {
            this.filePath = filePath;
            this.logger = logger;
            this.vault = vault;
            this.configuration = configuration;
            inheritedFamilyName = $"Pfingsten#{new Random().Next()}";
            people = new SortedSet<Person>();
            partnerships = new HashSet<Partnership>();
            this.logger.LogInformation("Beginning the Template Generation process.");
            nodeCollection = GetNodeCollection();
        }

        public FileStream GetTemplate()
        {
            IReadOnlyDictionary<HierarchialCoordinate, string> contents = BuildTemplate();
            if (filePath is null)
            {
                throw new NotImplementedException("I haven't gotten to databases yet.");
            }
            using FileStream stream = new(DEFAULT_WRITE_FILE_PATH, FileMode.Create, FileAccess.Write);
            using PdfWriter writer = new(stream);
            using PdfDocument document = new(writer);
            using Document template = new(document);
            const float representationHeight = 36f;
            float pageHeight = template.GetPageEffectiveArea(PageSize.A4).GetHeight();
            float currentHeight = 0f;
            foreach (KeyValuePair<HierarchialCoordinate, string> content in contents)
            {
                if (currentHeight + representationHeight > pageHeight)
                {
                    template.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    currentHeight = 0f;
                }
                template.Add(new Paragraph($"{content.Key} {content.Value}"));
                currentHeight += representationHeight;
            }
            template.Close();
            document.Close();
            writer.Close();
            return new(stream.Name, FileMode.Open, FileAccess.Read);
        }

        private IReadOnlyDictionary<HierarchialCoordinate,string> BuildTemplate()
        {
            return new SortedDictionary<HierarchialCoordinate, string>(nodeCollection.Select((node) => 
            {
                string representation = $"{people.First((person) => person.Id == node.Value.MemberId)}";
                if (node.Value.InLawId.HasValue)
                {
                    representation += $" & {people.First((person) => person.Id == node.Value.InLawId.Value)}:";
                    if (node.Value.PartnershipId.HasValue)
                    {
                        representation += $" {partnerships.First((partnership) => partnership.Id == node.Value.PartnershipId.Value)}";
                    }
                }
                return new KeyValuePair<HierarchialCoordinate, string>(node.Key, representation);
            }).ToDictionary());
        }

        private void FillNodeCollection(HierarchialCoordinate current, int g, Regex[] generationNotations, string content, string inheritedFamilyName, ref SortedDictionary<HierarchialCoordinate, FamilyTreeNode> nodes)
        {
            logger.LogDebug($"Considering generation {g + 1}");
            string[] parts = generationNotations[g].Split(content);
            if (g < generationNotations.Length - 1)
            {
                HierarchialCoordinate sibling = current;
                foreach (string part in parts)
                {
                    if (part != "")
                    {
                        string[] subParts = generationNotations[g+1].Split(part);
                        FamilyTreeNode node = GetNode(subParts[0]);
                        nodes[sibling] = node;
                        logger.LogDebug($"{sibling} {node}");
                        logger.LogDebug($"Finding children of {node}");
                        if (subParts.Length > 1)
                        {
                            FillNodeCollection(sibling++, g++, generationNotations, part[subParts[0].Length..], inheritedFamilyName, ref nodes);
                        }
                        logger.LogDebug($"Moving on to the next sibling of {node}");
                        sibling = sibling.Sibling ?? throw new NullReferenceException("The Root doesn't have siblings.");
                    }
                }
            }
        }

        private FamilyTreeNode GetNode(string part)
        {
            logger.LogDebug("Analyzing the header.");
            string[] lines = part.Trim().Split('\n');
            if (lines.Length < 1 || lines.Length > 2)
            {
                throw new InvalidDataException("Unable to parse this node.");
            }
            IDictionary<string,BridgeInstance> member = new Dictionary<string,BridgeInstance>();
            Match memberNameMath = NameExpression().Match(lines[0]);
            member["birthName"] = new(memberNameMath.Value);
            logger.LogDebug($"Spotted Member name: {memberNameMath.Value}");
            MatchCollection memberDateCollection = DateExpression().Matches(lines[0]);
            logger.LogDebug($"Spotted Dates: {string.Join(',', memberDateCollection.Select((date) => date.Value))}");
            IReadOnlyList<FamilyTreeDate> memberDates = [.. memberDateCollection.Select((memberDate) => new FamilyTreeDate(memberDate.Value))];
            IDictionary<string,BridgeInstance> node;
            Person p_member;
            if (lines.Length == 2)
            {
                IDictionary<string,BridgeInstance> inLaw = new Dictionary<string,BridgeInstance>();
                Match inLawNameMatch = NameExpression().Match(lines[1]);
                inLaw["birthName"] = new(inLawNameMatch.Value);
                logger.LogDebug($"Spotted InLaw name: {memberNameMath.Value}");
                MatchCollection inLawDateCollection = DateExpression().Matches(lines[1]);
                logger.LogDebug($"Spotted Dates: {string.Join(',', memberDateCollection.Select((date) => date.Value))}");
                IReadOnlyList<FamilyTreeDate> inLawDates = [.. inLawDateCollection.Select((inLawDate) => new FamilyTreeDate(inLawDate.Value))];
                switch (inLawDates.Count)
                {
                    case 0:
                        inLaw["birthDate"] = new();
                        inLaw["deceasedDate"] = new();
                        break;
                    case 1:
                        inLaw["birthDate"] = inLawDates[0].Instance;
                        inLaw["deceasedDate"] = new();
                        break;
                    case 2:
                        inLaw["birthDate"] = inLawDates[0].Instance;
                        inLaw["deceasedDate"] = inLawDates[1].Instance;
                        break;
                    default: throw new InvalidDataException("Too many dates.");
                }
                switch (memberDates.Count)
                {
                    case 0:
                        member["birthDate"] = new();
                        member["deceasedDate"] = new();
                        break;
                    case 1:
                        member["birthDate"] = memberDates[0].Instance;
                        member["deceasedDate"] = new();
                        break;
                    case 2:
                        member["birthDate"] = memberDates[0].Instance;
                        member["deceasedDate"] = new();
                        break;
                    case 3:
                        member["birthDate"] = memberDates[0].Instance;
                        member["deceasedDate"] = memberDates[2].Instance;
                        break;
                    default: throw new InvalidDataException("Too many dates");
                }
                p_member = GetPerson(member);
                Person p_inLaw = GetPerson(inLaw);
                IDictionary<string,BridgeInstance> partnership = new Dictionary<string,BridgeInstance>()
                {
                    ["partnershipDate"] = memberDates.Count > 1 ? memberDates[1].Instance : new(),
                };
                Partnership p = new(partnership, true);
                partnerships.Add(p);
                node = new Dictionary<string,BridgeInstance>()
                {
                    ["inheritedFamilyNames"] = new([new(inheritedFamilyName)]),
                    ["memberId"] = new(p_member.Id.ToString()),
                    ["inLawId"] = new(p_inLaw.Id.ToString()),
                    ["partnershipId"] = new(p.Id.ToString())
                };
                return new(node, true);
            }
            switch (memberDates.Count)
            {
                case 0:
                    member["birthDate"] = new();
                    member["deceasedDate"] = new();
                    break;
                case 1:
                    member["birthDate"] = memberDates[0].Instance;
                    member["deceasedDate"] = new();
                    break;
                case 2:
                    member["birthDate"] = memberDates[0].Instance;
                    member["deceasedDate"] = memberDates[1].Instance;
                    break;
                default: throw new InvalidDataException("Too many dates");
            }
            p_member = GetPerson(member);
            node = new Dictionary<string,BridgeInstance>()
            {
                ["inheritedFamilyNames"] = new([new(inheritedFamilyName)]),
                ["memberId"] = new(p_member.Id.ToString())
            };
            return new(node, true);
        }

        private IReadOnlyDictionary<HierarchialCoordinate,FamilyTreeNode> GetNodeCollection()
        {
            SortedDictionary<HierarchialCoordinate,FamilyTreeNode> nodes = [];
            string? extension = System.IO.Path.GetExtension(filePath);
            Regex[] generationNotations = [RomanUpper(), Upper(), Digit(), Lower(), ParenthesizedDigit(), RomanLower()];
            if (extension == "txt")
            {
                logger.LogDebug($"Reading from {DEFAULT_FILE_PATH}.");
                string content = File.ReadAllText(filePath ?? DEFAULT_FILE_PATH);
                logger.LogDebug(content);
                FillNodeCollection(new HierarchialCoordinate([1]), 0, generationNotations, content, inheritedFamilyName, ref nodes);
            }
            return nodes;
        }

        private Person GetPerson(IDictionary<string,BridgeInstance> personObj)
        {
            Person initial = new(personObj, true);
            bool isAdded = people.Add(initial);
            if (!isAdded)
            {
                return people.First(other => other == initial);
            }
            return initial;
        }

        [GeneratedRegex(@"([A-Z][a-z]*)(\s[A-Z][a-z\-\,\.]*)*", RegexOptions.Compiled)]
        private static partial Regex NameExpression();
        [GeneratedRegex(@"((\d\d|\d)\s([A-Z][a-z][a-z])\s(\d\d\d\d|\d\d\d\d\-\d\d\d\d))|(([A-Z][a-z][a-z])\s(\d\d\d\d|\d\d\d\d\-\d\d\d\d))|(\d\d\d\d|\d\d\d\d\-\d\d\d\d)", RegexOptions.Compiled)]
        public static partial Regex DateExpression();

        [GeneratedRegex(@"(I|V|X|L|C|D|M)+:\s", RegexOptions.Compiled)]
        private static partial Regex RomanUpper();

        [GeneratedRegex(@"([A-Z])+.\s", RegexOptions.Compiled)]
        private static partial Regex Upper();
        [GeneratedRegex(@"(\d)+.\s", RegexOptions.Compiled)]
        private static partial Regex Digit();
        [GeneratedRegex(@"([a-z])+.\s", RegexOptions.Compiled)]
        private static partial Regex Lower();
        [GeneratedRegex(@"\((\d)+\)\s", RegexOptions.Compiled)]
        private static partial Regex ParenthesizedDigit();
        [GeneratedRegex(@"(i|v|x|l|c|d|m)+\)\s", RegexOptions.Compiled)]
        private static partial Regex RomanLower();
    }
}
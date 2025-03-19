using FamilyTreeLibrary.Data.Models;
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
    public class TemplateGenerator : ITemplateGenerator
    {
        private readonly string inheritedFamilyName;
        private const string LOWER = @"^[a-z]+\.\s";
        private const string NUMERICAL = @"^\d+\.\s";
        private const string PARENTHESIZED_NUMERICAL = @"^\(\d+\)\s";
        private const string ROMAN_LOWER = @"^[ivxlcdm]+\)\s";
        private const string ROMAN_UPPER = @"^[IVXLCDM]+:\s";
        private const string UPPER = @"^[A-Z]+\.\s";
        private const string DATE_PATTERN = @"(?:\s+(?:(\d{1,2}\s)?[A-Z][a-z]{2}\s)?\d{4}(?:-\d{4})?)";
        private const string NAME_PATTERN = @"[A-Z][A-Za-z\-\,\.]*(?:\s[A-Z][A-Za-z\-\,\.]*)*";
        private readonly IReadOnlyDictionary<HierarchialCoordinate, FamilyTreeNode> family;
        private readonly IList<Models.Line> lines;
        private readonly ISet<Person> people;
        private readonly ISet<Partnership> partnerships;
        private readonly IExtendedLogger<TemplateGenerator> logger;

        public TemplateGenerator(IExtendedLogger<TemplateGenerator> logger)
        {
            this.logger = logger;
            int id = new Random().Next();
            inheritedFamilyName = $"Pfingsten#{id}";
            people = new SortedSet<Person>();
            partnerships = new HashSet<Partnership>();
            lines = [];
            family = GetFamily();
        }

        public TemplateGenerator(IExtendedLogger<TemplateGenerator> logger, int id)
        {
            this.logger = logger;
            inheritedFamilyName = $"Pfingsten#{id}";
            people = new SortedSet<Person>();
            partnerships = new HashSet<Partnership>();
            lines = [];
            family = GetFamily();
        }

        public IReadOnlyDictionary<HierarchialCoordinate, FamilyTreeNode> Family
        {
            get => family;
        }

        private static IDictionary<string,BridgeInstance> DefaultPerson
        {
            get
            {
                return new Dictionary<string,BridgeInstance>()
                {
                    ["birthName"] = new(),
                    ["birthDate"] = new(),
                    ["deceasedDate"] = new()
                };
            }
        }
        private static string FilePath
        {
            get
            {
                DirectoryInfo directory = new(Directory.GetCurrentDirectory());
                while(directory.Name != "FamilyTreeProject")
                {
                    directory = directory.Parent!;
                }
                return System.IO.Path.Combine(directory.FullName, "resources\\PfingstenFamilyAlternative.txt");
            }
        }

        private static string WritePath
        {
            get
            {
                DirectoryInfo directory = new(Directory.GetCurrentDirectory());
                while(directory.Name != "FamilyTreeProject")
                {
                    directory = directory.Parent!;
                }
                return System.IO.Path.Combine(directory.FullName, "resources\\2023PfingstenBookAlternate.pdf");
            }
        }

        public FileStream WriteTemplate()
        {
            logger.LogInformation("Writing partnerships to the path: \"{filePath}\"", WritePath);
            using FileStream initialStream = new(WritePath, FileMode.Create);
            using PdfWriter templateWriter = new(initialStream);
            using PdfDocument templateDocument = new(templateWriter);
            using Document template = new(templateDocument);
            const float representationHeight = 36f;
            float pageHeight = template.GetPageEffectiveArea(PageSize.A4).GetHeight();
            float currentHeight = 0f;
            foreach (Models.Line line in lines)
            {
                if (currentHeight + representationHeight > pageHeight)
                {
                    logger.LogDebug("Moving to a new page.");
                    template.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    currentHeight = 0f;
                }
                template.Add(new Paragraph(line.ToString()));
                logger.LogDebug("{line} has been written.", line);
                currentHeight += representationHeight;
            }
            template.Close();
            templateDocument.Close();
            templateWriter.Close();
            logger.LogInformation("Go ahead and look at: \"{filePath}\"", WritePath);
            return new(initialStream.Name, FileMode.Open, FileAccess.Read);
        }

        private Models.Line BuildHeader(Queue<string> whole, string orderTypePattern, HierarchialCoordinate coordinate)
        {
            string subOrderTypePattern = orderTypePattern switch {
                LOWER => PARENTHESIZED_NUMERICAL,
                NUMERICAL => LOWER,
                PARENTHESIZED_NUMERICAL => ROMAN_LOWER,
                ROMAN_UPPER => UPPER,
                UPPER => NUMERICAL,
                _ => ""
            };
            bool endOfLine = false;
            string memberPattern = GetMemberPattern(orderTypePattern);
            string subMemberPattern = GetMemberPattern(subOrderTypePattern);
            string inLawPattern = $@"^({NAME_PATTERN})\s*(({DATE_PATTERN})\s*)*\s*$";
            IDictionary<string,BridgeInstance> memberObj = DefaultPerson;
            IDictionary<string,BridgeInstance> inLawObj = DefaultPerson;
            IDictionary<string,BridgeInstance> partnershipObj = new Dictionary<string,BridgeInstance>();
            Queue<FamilyTreeDate> memberDates = new();
            Queue<FamilyTreeDate> inLawDates = new();
            while(!endOfLine && whole.TryDequeue(out string? text) && text is not null)
            {
                Match memberMatch = Regex.Match(text, memberPattern, RegexOptions.Compiled);
                if (memberMatch.Success)
                {
                    memberObj["birthName"] = new(memberMatch.Groups[1].Value);
                    logger.LogInformation("Member Name Found: {name}", memberObj["birthName"]);
                    memberDates = new(Regex.Matches(text,DATE_PATTERN, RegexOptions.Compiled).Cast<Match>().Select(m => new FamilyTreeDate(m.Value)));
                    if (memberDates.TryDequeue(out FamilyTreeDate? memberBirthDate) && memberBirthDate is not null)
                    {
                        memberObj["birthDate"] = memberBirthDate.Instance;
                        logger.LogInformation("Member Birth Date Found: {birthDate}", memberObj["birthDate"]);
                    }
                    else
                    {
                        logger.LogWarning("No birth date found for the member \"{name}\".", memberObj["birthName"]);
                    }
                    if (memberDates.Count == 2)
                    {
                        memberDates.Enqueue(memberDates.Dequeue());
                        memberObj["deceasedDate"] = memberDates.Dequeue().Instance;
                        logger.LogInformation("Member Deceased Date Found: {deceasedDate}", memberObj["deceasedDate"]);
                    }
                }
                else
                {
                    Match inLawMatch = Regex.Match(text, inLawPattern, RegexOptions.Compiled);
                    if (inLawMatch.Success)
                    {
                        inLawObj["birthName"] = new(inLawMatch.Groups[1].Value);
                        logger.LogInformation("InLaw Birth Name Found: {name}", inLawObj["birthName"]);
                        inLawDates = new(Regex.Matches(text,DATE_PATTERN, RegexOptions.Compiled).Cast<Match>().Select(m => new FamilyTreeDate(m.Value)));
                        if (inLawDates.Count <= 2 && inLawDates.Count > 0)
                        {
                            inLawObj["birthDate"] = inLawDates.Dequeue().Instance;
                            logger.LogInformation("InLaw Birth Date Found: {birthDate}", inLawObj["birthDate"]);
                        }
                        else
                        {
                            logger.LogWarning("No birth date found for the inLaw \"{name}\".", inLawObj["birthName"]);
                        }
                        if (inLawDates.Count == 1)
                        {
                            inLawObj["deceasedDate"] = inLawDates.Dequeue().Instance;
                            logger.LogInformation("InLaw Deceased Date Found: {deceasedDate}", inLawObj["deceasedDate"]);
                        }
                        if (memberDates.TryDequeue(out FamilyTreeDate? partnershipDate) && partnershipDate is not null)
                        {
                            partnershipObj["partnershipDate"] = partnershipDate.Instance;
                            logger.LogInformation("Partnership date found {partnershipDate}", partnershipObj["partnershipDate"]);
                        }
                        else
                        {
                            logger.LogWarning("No partnership date between {member} and {inLaw} found.", memberObj["birthName"], inLawObj["birthName"]);
                        }

                    }
                }
                endOfLine = !whole.TryPeek(out string? nextText) || nextText is null || Regex.IsMatch(nextText, subMemberPattern, RegexOptions.Compiled)
                    || Regex.IsMatch(nextText, memberPattern, RegexOptions.Compiled);
                logger.LogDebug("Done Reading Header? {endOfLine}", endOfLine);
            }
            if (memberDates.Count == 1)
            {
                memberObj["deceasedDate"] = memberDates.Dequeue().Instance;
                logger.LogInformation("Member Deceased Date Found: {deceasedDate}", memberObj["deceasedDate"]);
            }
            if (!IsPerson(memberObj))
            {
                throw new InvalidOperationException("The member must exist.");
            }
            Person member = GetPerson(new(memberObj, true));
            logger.LogDebug("{member}", member);
            Person? inLaw = IsPerson(inLawObj) ? GetPerson(new(inLawObj, true)) : null;
            logger.LogDebug("{inLaw}", inLaw);
            Partnership? partnership = IsPartnership(partnershipObj) ? GetPartnership(new(partnershipObj, true)) : null;
            logger.LogDebug("{partnership}", partnership);
            return new(coordinate, member, inLaw, partnership);
        }

        private Queue<Content> GetContents(Queue<string> whole, string[] orderTypePatterns, int i, HierarchialCoordinate start)
        {
            Queue<Content> contents = new();
            Content currentContent;
            HierarchialCoordinate currentCoordinate = start;
            Queue<string> subQueue = new();
            Models.Line header = new(new(), new(DefaultPerson, true));
            bool processHeader = true;
            while (whole.Count > 0)
            {
                if (processHeader)
                {
                    header = BuildHeader(whole, orderTypePatterns[i], currentCoordinate.Copy());
                    logger.LogDebug("{header}",header);
                    currentCoordinate = currentCoordinate.Sibling!.Value;
                    processHeader = !processHeader;
                }
                string pattern = GetMemberPattern(orderTypePatterns[i]);
                if (whole.TryPeek(out string? text) && text is not null && !Regex.IsMatch(text, pattern, RegexOptions.Compiled))
                {
                    logger.LogDebug("{header} has a child {text}.", header, text);
                    subQueue.Enqueue(whole.Dequeue());
                }
                else
                {
                    logger.LogDebug("All children of {header} has been processed.", header);
                    currentContent = new(header.Copy(),new(subQueue));
                    contents.Enqueue(currentContent.Copy());
                    subQueue.Clear();
                    processHeader = !processHeader;
                }
            }
            if (header.Member["birthName"].IsString && subQueue.Count > 0)
            {
                logger.LogDebug("Considering the last child of {header}", header);
                currentContent = new(header, subQueue);
                contents.Enqueue(currentContent);
            }
            return contents;
        }

        private IReadOnlyDictionary<HierarchialCoordinate, FamilyTreeNode> GetFamily()
        {
            SortedDictionary<HierarchialCoordinate, FamilyTreeNode> family = [];
            Stack<Queue<Content>> contents = new();
            string[] orderTypePatterns = [ROMAN_UPPER, UPPER, NUMERICAL, LOWER, PARENTHESIZED_NUMERICAL, ROMAN_LOWER];
            logger.LogInformation("Retrieving the partnerships of {inheritedFamilyName}.", inheritedFamilyName);
            Queue<string> lines = ReadTextFile();
            contents.Push(GetContents(lines, orderTypePatterns, contents.Count, new([1])));
            while (contents.TryPop(out Queue<Content>? collection) && collection is not null)
            {
                if (collection.TryDequeue(out Content current))
                {
                    Content content = current.Copy();
                    logger.LogDebug("{header} is ready for writing.", content.Header);
                    this.lines.Add(content.Header);
                    logger.LogDebug("Constructing a node associating {header}", content.Header);
                    IDictionary<string,BridgeInstance> nodeObj = new Dictionary<string,BridgeInstance>()
                    {
                        ["inheritedFamilyNames"] = new([new(inheritedFamilyName)]),
                        ["memberId"] = new(content.Header.Member.Id.ToString()),
                        ["inLawId"] = content.Header.InLaw is null ? new() : new(content.Header.InLaw.Id.ToString()),
                        ["partnershipId"] = content.Header.Partnership is null ? new() : new(content.Header.Partnership.Id.ToString())
                    };
                    family[content.Header.Coordinate] = new(nodeObj, true);
                    logger.LogDebug("{coordinate}: {node}", content.Header.Coordinate, family[content.Header.Coordinate]);
                    contents.Push(collection);
                    contents.Push(GetContents(content.SubContent, orderTypePatterns, contents.Count, content.Header.Coordinate.Child));
                }
            }
            return family;
        }

        private static string GetMemberPattern(string orderTypePattern)
        {
            return $@"{orderTypePattern}({NAME_PATTERN})\s*(({DATE_PATTERN})\s*)*\s*$";
        }

        private Partnership GetPartnership(Partnership p)
        {
            if (partnerships.Add(p))
            {
                return p;
            }
            return partnerships.First(temp => temp == p);
        }

        private Person GetPerson(Person p)
        {
            if (people.Add(p))
            {
                return p;
            }
            return people.First((temp) => temp == p);
        }

        private static bool IsPartnership(IDictionary<string,BridgeInstance> obj)
        {
            return obj.ContainsKey("partnershipDate");
        }

        private static bool IsPerson(IDictionary<string,BridgeInstance> obj)
        {
            return obj.TryGetValue("birthName", out BridgeInstance birthName) && birthName.IsString;
        }

        public Queue<string> ReadTextFile()
        {
            logger.LogInformation("Reading from {FilePath}.", FilePath);
            return new(File.ReadLines(FilePath));
        }
    }
}
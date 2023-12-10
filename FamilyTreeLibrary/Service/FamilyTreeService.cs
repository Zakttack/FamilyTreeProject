using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Data.PDF;
using FamilyTreeLibrary.Models;
using Serilog;

namespace FamilyTreeLibrary.Service
{
    public class FamilyTreeService
    {
        public FamilyTreeService(string familyName)
        {
            FamilyTreeUtils.InitializeLogger();
            FamilyTree = new Tree(familyName);
        }

        public int NumberOfGenerations
        {
            get
            {
                return FamilyTree.Height;
            }
        }

        public long NumberOfFamilies
        {
            get
            {
                return FamilyTree.Count;
            }
        }

        public void AppendTree(string templateFilePath)
        {
            try
            {
                PdfClient client = new(templateFilePath);
                client.LoadNodes();
                client.AttachNodes();
                if (client.Nodes.Any() && client.Nodes.First().Parent is not null)
                {
                    throw new InvalidOperationException("The root must be the first node to be analyzed.");
                }
                foreach (FamilyNode node in client.Nodes)
                {
                    if ((node.Parent is null && !FamilyTree.Any()) || !FamilyTree.Contains(node.Parent, node.Element))
                    {
                        Log.Debug($"Adding {node}");
                        FamilyTree.Add(node.Parent, node.Element);
                        Log.Debug($"{node} has been added.");
                    }
                    Log.Information($"{node} exists in the tree.");
                }
                long pdfNodesCount = client.Nodes.LongCount();
                long treeNodesCount = FamilyTree.Count;
                if (pdfNodesCount != treeNodesCount)
                {
                    Log.Warning($"Only {treeNodesCount} of {pdfNodesCount} were analyzed.");
                }
                else
                {
                    Log.Information("All nodes have been analyzed.");
                }
            }
            catch (IOException ex)
            {
                throw new FileNotFoundException("The provided file isn't found in your directory. Try to upload again.", ex);
            }
        }

        public void ReportChildren(Family parent, Family child)
        {
            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent), $"The child has to be a parent of somebody in the {FamilyTree.Name} family tree.");
            }
            else if (child is null)
            {
                throw new ArgumentNullException(nameof(child), "The child you're trying to report doesn't exist.");
            }
            else if (!FamilyTree.Any(node => node == parent) && !FamilyTree.Any(node => node == child))
            {
                throw new ArgumentException($"Either {parent.Member.Name} or {child.Member.Name} must be genetically part of the {FamilyTree.Name} Family Tree.");
            }
            else if (FamilyTree.Contains(parent, child))
            {
                throw new InvalidOperationException($"{parent.Member.Name} already has a child named {child.Member.Name}");
            }
            Log.Information($"Adding the parent-child relationship between {parent.Member.Name} and {child.Member.Name}");
            FamilyTree.Add(parent, child);
            Log.Information($"{parent.Member.Name} has a child named {child.Member.Name}");
        }

        public void ReportDeceased(Person p, FamilyTreeDate deceasedDate)
        {
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p), "The person you're trying to report doesn't exist.");
            }
            else if (deceasedDate == default || deceasedDate == FamilyTreeDate.DefaultDate)
            {
                throw new ArgumentNullException(nameof(deceasedDate), "The deceased date provided doesn't exist.");
            }
            IEnumerable<Family> families = FamilyTree.Where((node) => node.Member == p || node.InLaw == p);
            if (!families.Any())
            {
                throw new ArgumentException($"{p.Name} isn't found in the tree.");
            }
            foreach (Family fam in families)
            {
                Family updatedFam = fam;
                if (updatedFam.Member == p)
                {
                    updatedFam.Member.DeceasedDate = deceasedDate;
                }
                else if (updatedFam.InLaw == p)
                {
                    updatedFam.InLaw.DeceasedDate = deceasedDate;
                }
                FamilyTree.Update(fam, updatedFam);
            }
        }

        public void ReportMarried(Person member, Person inLaw, FamilyTreeDate marriageDate)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member), "The biological member you're trying to report doesn't exist.");
            }
            else if (inLaw is null)
            {
                throw new ArgumentNullException(nameof(inLaw), $"{member.Name} can't married to someone who doesn't exist.");
            }
            else if (marriageDate == default || marriageDate == FamilyTreeDate.DefaultDate)
            {
                throw new ArgumentNullException(nameof(marriageDate), "An unknown marriage date was entered.");
            }
            IEnumerable<Family> families = FamilyTree.Where(node => node.Member == member);
            if (!families.Any())
            {
                throw new ArgumentException($"{member.Name} isn't a member of the tree.");
            }
            Family family = families.Where(fam => fam.InLaw == inLaw).FirstOrDefault();
            if (family is null && (families.Count() > 1 || families.First().InLaw is not null))
            {
                Family additionalFamily = new(member, inLaw, marriageDate);
                Family node = FamilyTree.Where(n => n.Member == member).First();
                Family parent = FamilyTree.GetParent(node);
                FamilyTree.Add(parent, additionalFamily);
            }
            else if (family is null)
            {
                Family initial = FamilyTree.Where(node => node == families.First()).First();
                Family final = initial;
                final.InLaw = inLaw;
                final.MarriageDate = marriageDate;
                FamilyTree.Update(initial, final);
            }
            else
            {
                Family initial = FamilyTree.Where(node => node == family).First();
                Family final = initial;
                final.MarriageDate = marriageDate;
                FamilyTree.Update(initial, final);
            }
        }

        private ITree FamilyTree
        {
            get;
        }
    }
}
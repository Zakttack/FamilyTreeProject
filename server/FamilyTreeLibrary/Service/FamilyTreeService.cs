using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Data.Comparers;
using FamilyTreeLibrary.Data.PDF;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using Serilog;

namespace FamilyTreeLibrary.Service
{
    public class FamilyTreeService
    {
        public FamilyTreeService(string familyName)
        {
            FamilyTree = new Tree(familyName);
        }

        public IEnumerable<Family> AscendingByName
        {
            get
            {
                return FamilyTree.Order(new NameAscedendingComparer());
            }
        }

        public int NumberOfGenerations
        {
            get
            {
                return FamilyTree.Height - 1;
            }
        }

        public long NumberOfFamilies
        {
            get
            {
                return FamilyTree.Count - 1;
            }
        }

        public IEnumerable<Family> ParentFirstThenChildren
        {
            get
            {
                return FamilyTree;
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
                    string parentPortion;
                    if (node.Parent is null)
                    {
                        parentPortion = "null";
                    }
                    else if (node.Parent.Member.Name is null)
                    {
                        parentPortion = "root";
                    }
                    else
                    {
                        parentPortion = node.Parent.InLaw is null ? node.Parent.Member.Name : $"{node.Parent.Member.Name}/{node.Parent.InLaw.Name}";
                    }
                    string childPortion;
                    if (node.Element.Member.Name is null)
                    {
                        childPortion = "root";
                    }
                    else
                    {
                        childPortion = node.Element.InLaw is null ? node.Element.Member.Name : $"{node.Element.Member.Name}/{node.Element.InLaw.Name}";
                    }
                    if ((node.Parent is null && !FamilyTree.Any()) || !FamilyTree.Contains(node.Parent, node.Element))
                    {
                        Log.Debug($"Adding the parent-child relationship between {parentPortion} and {childPortion}.");
                        FamilyTree.Add(node.Id, node.Parent, node.Element);
                        Log.Debug($"The parent-child relationship between {parentPortion} and {childPortion} has been added.");
                    }
                    Log.Information($"The parent-child relationship between {parentPortion} and {childPortion} exists in the tree.");
                }
                long pdfNodesCount = client.Nodes.LongCount();
                long treeNodesCount = FamilyTree.Count;
                if (pdfNodesCount != treeNodesCount)
                {
                    Log.Warning($"Only {treeNodesCount} of {pdfNodesCount} were analyzed.");
                }
                else
                {
                    Log.Information($"{treeNodesCount} nodes have been analyzed.");
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
            FamilyTree.Add(default, parent, child);
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
                Family updatedFam = (Family)fam.Clone();
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
            IList<Family> familyCollection = FamilyTree.ToList();
            IList<Family> families = familyCollection.Where(node => node.Member == member).ToList();
            if (!families.Any())
            {
                throw new ArgumentException($"{member.Name} isn't a member of the tree.");
            }
            Family family = families.Where(fam => fam.InLaw == inLaw).FirstOrDefault();
            if (family is null && (families.Count > 1 || families[0].InLaw is not null))
            {
                Family additionalFamily = new(member, inLaw, marriageDate);
                Family node = (Family)families[0].Clone();
                Family parent = FamilyTree.GetParent(node);
                Log.Debug("An additional partnership is being added.");
                FamilyTree.Add(default, parent, additionalFamily);
                Log.Information("The additional partnership has been applied.");
            }
            else if (family is null)
            {
                Family initial = familyCollection.Where(node => node == families[0]).First();
                Family final = new(initial.Member, inLaw, marriageDate);
                Log.Debug($"The node: {initial} is being updated.");
                FamilyTree.Update(initial, final);
                Log.Information($"The node has been updated to {final}");
            }
            else
            {
                Family initial = familyCollection.Where(node => node == family).First();
                Family final = new(initial.Member, initial.InLaw, marriageDate);
                Log.Debug($"The marriage date is being changed from {initial.MarriageDate} to {final.MarriageDate}.");
                FamilyTree.Update(initial, final);
                Log.Information("The marriage date update was successful");
            }
        }

        public Family RetrieveParentOf(Family element)
        {
            Family parent = FamilyTree.GetParent(element);
            if (parent is null || parent == Family.EmptyFamily)
            {
                throw new FamilyNotFoundException($"{element.Member.Name} has an unknown parent.");
            }
            return parent;
        }

        public void RevertTree(string templateFilePath)
        {
            FamilyTree.Clear();
            AppendTree(templateFilePath);
        }

        private ITree FamilyTree
        {
            get;
        }
    }
}
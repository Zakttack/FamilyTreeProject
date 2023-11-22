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
            Tree = new FamilyTree(familyName);
        }

        public void AppendTree(string templateFilePath)
        {
            try
            {
                PdfClient client = new(templateFilePath);
                client.LoadNodes();
                client.AttachNodes();
                foreach (Family node in client.Nodes)
                {
                    if (!Tree.Contains(node))
                    {
                        Log.Debug($"Adding {node}");
                        Family parent = Tree[node.Parent];
                        Tree.Add(parent, node);
                        Log.Debug($"{node} has been added.");
                    }
                    Log.Information($"{node} exists in the tree.");
                }
                long pdfNodesCount = client.Nodes.LongCount();
                long treeNodesCount = Tree.Count;
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

        public void ReportChildren(Family parent, Person child)
        {
            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent), $"The child has to be a parent of somebody in the {Tree.Name} family tree.");
            }
            else if (!Tree.Contains(parent))
            {
                string message = $"{parent.Member.Name} " + parent.InLaw is null ? "is " : $" and {parent.InLaw.Name} are " + $"not part of the {Tree.Name} family tree."; 
                throw new ArgumentException(message);
            }
            else if (child is null)
            {
                throw new ArgumentNullException(nameof(child), "The child you're trying to report doesn't exist.");
            }
            Family childNode = new(child)
            {
                Parent = parent.Member,
                InLaw = null,
                MarriageDate = FamilyTreeDate.DefaultDate
            };
            Tree.Add(parent, childNode);
        }

        public void ReportDeceased(Person p, FamilyTreeDate deceasedDate)
        {
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p), "The person you're trying to report doesn't exist.");
            }
            else if (deceasedDate == default)
            {
                throw new ArgumentNullException(nameof(deceasedDate), "The deceased date provided doesn't exist.");
            }
            IEnumerable<Family> families = Tree.Where((node) => node.Member == p || node.InLaw == p);
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
                else
                {
                    updatedFam.InLaw.DeceasedDate = deceasedDate;
                }
                Tree.Update(fam, updatedFam);
            }
        }

        public void ReportMarried(Person parent, Person member, Person inLaw, FamilyTreeDate marriageDate)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member), "The biological member you're trying to report doesn't exist.");
            }
            else if (inLaw is null)
            {
                throw new ArgumentNullException(nameof(inLaw), $"{member.Name} can't married to someone who doesn't exist.");
            }
            else if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent), "The parent doesn't exist.");
            }
            Family parentNode = Tree[parent] ?? throw new ArgumentException($"{parent.Name} isn't found in the tree.");
            IEnumerable<Family> families = Tree.Where((fam) => fam.Parent == parent && fam.Member == member && fam.InLaw == inLaw);
            Family family;
            if (families.Any())
            {
                family = Tree[member];
                family.MarriageDate = marriageDate;
            }
            else
            {
                family = new(member)
                {
                    Parent = parent,
                    InLaw = inLaw,
                    MarriageDate = marriageDate
                };
                Tree.Add(parentNode, family);
            }
        }



        private IFamilyTree Tree
        {
            get;
        }
    }
}
using FamilyTreeLibrary;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;
using iText.Signatures;
using Serilog;

// const string FAMILY_NAME = "Pfingsten";
// const string PDF_FILE = "2023PfingstenBookAlternate.pdf";
// try
// {
//     string filePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), PDF_FILE);
//     FamilyTreeService treeService = new(FAMILY_NAME);
//     treeService.AppendTree(filePath);
// }
// catch (Exception ex)
// {
//     Log.Fatal($"Something went wrong. Check below for further info:\n {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}", ex);
// }

// static void PrintDuplicates(IEnumerable<Family> collection)
// {
//     IDictionary<Family,ICollection<Family>> treeNodeDuplicateAnalyzer = new SortedDictionary<Family,ICollection<Family>>();
//     foreach (Family treeNode in collection)
//     {
//         if (treeNodeDuplicateAnalyzer.ContainsKey(treeNode))
//         {
//             treeNodeDuplicateAnalyzer[treeNode].Add(treeNode);
//         }
//         else
//         {
//             treeNodeDuplicateAnalyzer.Add(new(treeNode, new List<Family>() {treeNode}));
//         }
//     }
//     IEnumerable<KeyValuePair<Family,ICollection<Family>>> duplicateAnalyzer = treeNodeDuplicateAnalyzer.Where((entry) => entry.Value.Count > 1);
//     IEnumerable<Family> duplicates = new List<Family>();
//     foreach (KeyValuePair<Family,ICollection<Family>> entry in duplicateAnalyzer)
//     {
//         IEnumerable<Family> initial = duplicates;
//         duplicates = initial.Concat(entry.Value);
//     }
//     foreach (Family duplicate in duplicates)
//     {
//         Console.WriteLine(duplicate);
//     }
// }


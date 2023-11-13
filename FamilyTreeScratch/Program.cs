using FamilyTreeLibrary;
using FamilyTreeLibrary.Service;
using Serilog;

const string FAMILY_NAME = "Pfingsten";
const string PDF_FILE = "2023PfingtenBookAlternate.pdf";
FamilyTreeService service = new(FAMILY_NAME);
string templateFilePath = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), PDF_FILE);
try
{
    service.AppendTree(templateFilePath);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Something went wrong. See the further information below");
}
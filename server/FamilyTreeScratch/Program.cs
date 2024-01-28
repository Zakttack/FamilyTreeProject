using FamilyTreeLibrary;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;
using iText.Signatures;
using Serilog;

ITree tree = new Tree("Pfingsten");
foreach (Family family in tree)
{
    Console.WriteLine(family);
}


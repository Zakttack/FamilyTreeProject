using FamilyTreeLibrary;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;
using iText.Signatures;
using Serilog;

string line = "Gabriel Jose Thompson-Guzman (15 Jan. 2016 - Present)";
string[] separator = new string[] {" (", " - ", ")"};
string[] parts = line.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
foreach (string part in parts)
{
    Console.WriteLine(part);
}


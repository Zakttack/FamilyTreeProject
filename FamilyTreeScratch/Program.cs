using FamilyTreeLibrary.PDF;

const string PDF_FILE = "2023PfingtenBookAlternate.pdf";
PdfClient client = new(PDF_FILE);
Task nodeCreationTask = client.LoadNodes();
nodeCreationTask.Wait();
Console.WriteLine("Nodes are Loaded.");
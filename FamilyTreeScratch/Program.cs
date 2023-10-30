using FamilyTreeLibrary.PDF;

const string PDF_FILE = "2023PfingtenBookAlternate.pdf";
PdfClient client = new(PDF_FILE);
client.LoadNodes();
client.AttachNodes();
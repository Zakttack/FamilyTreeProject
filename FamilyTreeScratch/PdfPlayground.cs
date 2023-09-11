using FamilyTreeLibrary;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace FamilyTreeScratch
{
    public class PdfPlayground
    {
        public static IEnumerable<string> Content
        {
            get
            {
                PdfReader reader = new(FamilyTreeUtils.GetFileNameFromResources("PfingstenBook2023.pdf"));
                PdfDocument document = new (reader);
                ICollection<string> lines = new List<string>();
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                string page = PdfTextExtractor.GetTextFromPage(document.GetPage(1), strategy);
                string[] data = page.Split('\n');
                foreach (string d in data)
                {
                    lines.Add(d);
                }
                return lines;
            }
        }
    }
}
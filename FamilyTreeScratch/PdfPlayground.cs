using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace FamilyTreeScratch
{
    public class PdfPlayground
    {
        public static string PdfFilePath
        {
            get
            {
                return GetPdfFilePath(Directory.GetCurrentDirectory());
            }
        }

        public static bool FileExists
        {
            get
            {
                return File.Exists(PdfFilePath);
            }
        }

        public static IEnumerable<string> Content
        {
            get
            {
                PdfReader reader = new(PdfFilePath);
                PdfDocument document = new (reader);
                ICollection<string> lines = new List<string>();
                for (int p = 1; p <= document.GetNumberOfPages(); p++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string page = PdfTextExtractor.GetTextFromPage(document.GetPage(p), strategy);
                    string[] data = page.Split('\n');
                    foreach (string d in data)
                    {
                        lines.Add(d);
                    }
                }
                return lines;
            }
        }

        private static string GetPdfFilePath(string currentPath)
        {
            string[] parts = currentPath.Split('\\');
            string part = parts[^1];
            if (part != "FamilyTreeProject")
            {
                return GetPdfFilePath(currentPath[..(currentPath.Length - part.Length - 1)]);
            }
            return $"{currentPath}\\Resources\\PfingstenBook2023.pdf";
        }
    }
}
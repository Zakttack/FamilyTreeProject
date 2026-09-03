using System.Text;
using VirtualFamilyMuseumLibrary.Drive;
using VirtualFamilyMuseumLibrary.Drive.Models;

namespace VirtualFamilyMuseumScratch
{
    // Sketch of "Option 2": wrapping a TemplateLine into physical lines is fully
    // decoupled from packing physical lines into fixed-size pages. Neither phase
    // needs to predict what the other one will do.
    public static class DenormalizeSketch
    {
        private const int MAX_LINES_PER_PAGE = 49;
        private const int MAX_CHARACTERS_PER_LINE = 75;

        // Phase A: word-wrap ONE template line's rendered text. No knowledge of
        // pages here at all — it just keeps producing lines until the tokens run out.
        public static List<string> WrapLine(string text)
        {
            string[] tokens = text.Split();
            List<string> wrapped = [];
            StringBuilder lineBuilder = new();

            foreach (string token in tokens)
            {
                if (lineBuilder.Length == 0)
                {
                    lineBuilder.Append(token);
                }
                else if (lineBuilder.Length + 1 + token.Length <= MAX_CHARACTERS_PER_LINE)
                {
                    lineBuilder.Append(' ').Append(token);
                }
                else
                {
                    wrapped.Add(lineBuilder.ToString());
                    lineBuilder.Clear();
                    lineBuilder.Append(token);
                }
            }
            if (lineBuilder.Length > 0)
            {
                wrapped.Add(lineBuilder.ToString());
            }
            return wrapped;
        }

        // Phase B: pack an already-known sequence of physical lines into pages.
        // No estimating required — each line is placed one at a time, and capacity
        // is checked right before that placement, so it can never be wrong.
        public static IEnumerable<string[]> PackPages(IEnumerable<string> physicalLines)
        {
            List<string[]> pages = [];
            string[] page = new string[MAX_LINES_PER_PAGE];
            int linePosition = 0;

            foreach (string physicalLine in physicalLines)
            {
                if (linePosition >= page.Length)
                {
                    pages.Add(page);
                    page = new string[MAX_LINES_PER_PAGE];
                    linePosition = 0;
                }
                page[linePosition++] = physicalLine;
            }

            if (linePosition > 0)
            {
                pages.Add(page);
            }

            return pages;
        }

        public static IEnumerable<string[]> Denormalize(IEnumerable<TemplateLine> normalizedLines)
        {
            IEnumerable<string> allPhysicalLines = normalizedLines.SelectMany(line => WrapLine(line.ToString()));
            return PackPages(allPhysicalLines);
        }

        public static void Demo()
        {
            string[] sampleLines =
            [
                "1) Todd Solo (1975 – Present)",
                "1.1) Kit Dale Kessler (–)",
                "2) Colby Bryan Kessler (1888 – 1942)",
                "1) Brian Bryan Kessler (Sep 1886 – Dec 1915) & Todd Zachary Vasterling (1911 – Present): 1948",
            ];

            IEnumerable<TemplateLine> templateLines = sampleLines.Select(line => line.AsTemplateLine());
            IEnumerable<string[]> pages = Denormalize(templateLines);

            Console.WriteLine();
            Console.WriteLine("=== DenormalizeSketch.Demo ===");
            int pageNumber = 0;
            foreach (string[] page in pages)
            {
                pageNumber++;
                int written = page.Count(line => line is not null);
                Console.WriteLine($"--- Page {pageNumber} ({written} line(s) written of {page.Length}) ---");
                foreach (string? line in page)
                {
                    if (line is not null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }
    }
}

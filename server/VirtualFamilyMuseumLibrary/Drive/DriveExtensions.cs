using System.Text.RegularExpressions;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Models;

namespace VirtualFamilyMuseumLibrary.Drive
{
    public static class DriveExtensions
    {
        public static TemplateLine AsTemplateLine(this string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new InvalidCastException("An empty line can't be a template line.");
            }

            int memberInLawSeparatorIndex = line.IndexOf('&');
            int inLawFamilyDynamicSeparatorIndex = line.IndexOf(':');
            bool hasInLaw = memberInLawSeparatorIndex >= 0;
            bool hasFamilyDynamicStartDate = inLawFamilyDynamicSeparatorIndex >= 0;

            if (!hasInLaw && hasFamilyDynamicStartDate)
            {
                throw new InvalidCastException("In a template line, a family dynamic can only exist if an in-law exists.");
            }
            if (hasInLaw && hasFamilyDynamicStartDate && memberInLawSeparatorIndex > inLawFamilyDynamicSeparatorIndex)
            {
                throw new InvalidCastException("In a template line, the in-law summary must come before the family dynamic summary.");
            }

            // hasFamilyDynamicStartDate implies hasInLaw here — the mismatched case already threw above.
            char[] delimiters = hasFamilyDynamicStartDate
                ? ['(', ')', Extensions.EN_DASH, '&', ':']
                : hasInLaw
                    ? ['(', ')', Extensions.EN_DASH, '&']
                    : ['(', ')', Extensions.EN_DASH];

            // Split leaves a blank part at each "& " and "): " boundary (indices 4 and 8),
            // which is why the in-law and family dynamic fields start at 5 and skip to 9.
            string[] parts = [.. line.Split(delimiters).Select(part => part.Trim())];
            HierarchicalCoordinate coordinate = new([.. parts[0].Split('.').Select(int.Parse)]);

            return new TemplateLine
            {
                Coordinate = coordinate,
                MemberBirthName = parts[1],
                MemberBirthDate = FamilyDate.GetDate(parts[2]),
                MemberDeceasedDate = FamilyDate.GetDate(parts[3]),
                InLawBirthName = hasInLaw ? parts[5] : null,
                InLawBirthDate = hasInLaw ? FamilyDate.GetDate(parts[6]) : null,
                InLawDeceasedDate = hasInLaw ? FamilyDate.GetDate(parts[7]) : null,
                FamilyDynamicStartDate = hasFamilyDynamicStartDate ? FamilyDate.GetDate(parts[9]) : null
            };
        }
        public static FamilyDriveContainers GetContainer(string blobName)
        {
            string containerName = blobName.Split('/').First();
            return containerName switch
            {
                "images" => FamilyDriveContainers.Images,
                "templates" => FamilyDriveContainers.Templates,
                _ => throw new NotSupportedException($"{containerName} isn't part of the family drive.")
            };
        }

        public static string GetContentType(this FamilyContentTypes contentType)
        {
            return contentType.ToString().Replace('_', '/').ToLower();
        }

        public static FamilyContentTypes GetContentType(this string text)
        {
            return text switch
            {
                "application/pdf" => FamilyContentTypes.Application_PDF,
                "image/jpeg" => FamilyContentTypes.Image_JPEG,
                _ => throw new NotSupportedException($"{text} isn't a supported content-type.")
            };
        }

        public static Queue<string> GetPdfPageLines(string[] initialLines)
        {
            Regex hierarchicalCoordinateRegex = new(@"^\d+(\.\d+)*\)\s+[A-Za-z]", RegexOptions.Compiled);
            List<string> pdfLines = [.. initialLines];
            int index = 0;
            while (index < pdfLines.Count)
            {
                if (hierarchicalCoordinateRegex.IsMatch(pdfLines[index]))
                {
                    index++;
                }
                else
                {
                    pdfLines[index - 1] = $"{pdfLines[index - 1].Trim()} {pdfLines[index].Trim()}";
                    pdfLines.RemoveAt(index);
                }
            }
            return new Queue<string>(pdfLines.Select(line => line.Trim()));
        }
    }
}
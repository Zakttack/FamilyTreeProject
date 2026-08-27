## Family Drive
# Table Of Contents
1. Overview
2. Key Constants
3. Drive Extensions
4. Models
5. Template Processing
6. Repository

# Overview
This resource is home for holding versioned templates and images. This drive consists of 2 containers:  
- templates: holds read-only view versioned snapshot of family tree content relative to a single inherited family name stored as PDFs.
- images: photos of individuals or family dynamics stored as JPGs.

# Key Constants
These are the key constants needed for my .NET 9 library to know in order to upload, download and delete templates and images:  
- Blob Service Endpoint (`FamilyDrive:BlobServiceEndpoint`)
- Container names (`FamilyDrive:ImageContainerName` and `FamilyDrive:TemplateContainerName`): images and templates

# Drive Extensions
`DriveExtensions` holds the conventions every other piece of this module is built on:
- `GetContainer(string blobName)`: reads the first `/`-delimited segment of a blob name (e.g. `templates/2026/August/14/file.pdf`) and resolves it to the matching `FamilyDriveContainers` value. Every blob name handed to the Repository must be prefixed with `images/` or `templates/` so it's clear upfront which container it belongs to; anything else throws `NotSupportedException`.
- `GetContentType(this FamilyContentTypes)`: converts a content-type enum value into its MIME string (e.g. `Application_PDF` -> `application/pdf`) for setting a blob's HTTP headers on upload.
- `GetContentType(this string)`: the reverse mapping, turning a blob's stored MIME type back into a `FamilyContentTypes` value when reading a blob back out. Throws `NotSupportedException` for anything not in the supported set.

# Models
- `FamilyDriveContainers`: enum identifying which container a blob lives in (`Images`, `Templates`).
- `FamilyContentTypes`: enum of the content types this drive supports (`Application_PDF`, `Image_JPEG`). Adding a new supported file type means adding a value here plus a mapping on both sides of `DriveExtensions.GetContentType`.
- `FamilyBlobResource`: the DTO returned by every Repository operation. Carries `BlobName` (the container-prefixed logical name), `BlobUrl` (the blob's Azure URI), `Content` (the blob's readable `Stream`), and `ContentType`.

These three describe a blob as Azure sees it — which container it's in, what its bytes mean, how to fetch/store it — regardless of what's inside. `HierarchicalCoordinate` and `TemplateLine`, also under `Drive/Models`, describe what's *inside* a `templates` blob once you have one; see Template Processing below.

# Template Processing
This is a template-content concern, not a blob-storage concern: it's about modeling the PDF paragraphs found inside a blob stored in the `templates` container (content-type `application/pdf`), independent of how that blob was fetched or saved.

A template PDF renders one inherited family name's tree as a flat, depth-first-ordered list of paragraphs, each one either introducing a member (optionally alongside an in-law via a family dynamic — marriage, widowhood, divorce, co-parenting, etc.) or one of that member's children, e.g.:
```
1) Brian Bryan Kessler (Sep 1886 – Dec 1915) & Todd Zachary Vasterling (1911 – Present): 1948
1.1) Kit Dale Kessler (–)
1.2) Lisa Kaylynn Kessler (–) & Hallie Jordon Delacroix (21 Jan 1927 – Present): 11 May 1967
1.2.1) Kevin Nathaniel Kessler (21 Jan 1949 – Present) & August Hallie Aldous nee Bordt (9 Oct 1951 – Present): 22 May 1975
```
Sample trees illustrating this format live under `Resources/template_samples`.

- `HierarchicalCoordinate`: an immutable outline-numbering coordinate (`1)`, `1.1)`, `1.1.2)`, ...) — the vertex a `TemplateLine` occupies within the tree, in Graph Theory terms. Backed by an `int[]` of segments, one per generation of depth. `.ToString()` renders it exactly as it appears in a template (`1.1.2)`). `.Parent`/`.Child`/`.NextSibling` navigate the coordinate space itself to compute the next coordinate to assign while walking or building a tree — they don't touch any tree data. `IComparable`/operators order coordinates the same way the lines appear in the PDF: a coordinate always sorts before its own children, and siblings compare numerically per segment (`1.9)` before `1.10)`), not lexicographically. The all-empty coordinate is a virtual document root: `.Parent` and `.NextSibling` are `null`, `.ToString()` is `""`, and `.Child` yields `1)` — the first top-level entry.
- `TemplateLine`: one paragraph of a rendered template PDF — a member, positioned at a `HierarchicalCoordinate`, with an optional in-law and the date their family dynamic began. `Coordinate` and `MemberBirthName` are always required; `MemberBirthDate`/`MemberDeceasedDate` and `InLawBirthName`/`InLawBirthDate`/`InLawDeceasedDate`/`FamilyDynamicStartDate` are all optional. `.ToString()` renders a line back into the same text format the samples use — a missing birth date prints as `(–)`, a birth date with no deceased date prints as `(... – Present)`. The model assumes, without checking, that a deceased date is never set without a birth date (for both member and in-law), and that in-law dates and the family dynamic start date are never set without an in-law birth name — see the comment on the class. Enforcing those rules is domain/business logic and is deliberately left outside this model; where that validation should live is still an open decision.

Turning a template blob into a sequence of `TemplateLine`s happens in two steps, mirroring the two models above:

- `DriveExtensions.GetPdfPageLines(string[] initialLines)`: takes the raw lines iText extracts from a single PDF page (`PdfTextExtractor.GetTextFromPage`) and re-joins any that wrapped mid-paragraph back into one line per tree entry. A line only starts a new entry if it matches a hierarchical coordinate followed by whitespace and the start of a name (`^\d+(\.\d+)*\)\s+[A-Za-z]`); anything else is assumed to be the tail end of the previous line wrapping onto a new physical line and gets merged back on. That trailing-name requirement matters: without it, a wrapped date fragment like `1950)` or `2002):` looks coordinate-shaped too and gets misread as a new entry. Returns a `Queue<string>` so a consumer can drain normalized lines in the same top-to-bottom order they appeared on the page.
- `DriveExtensions.AsTemplateLine(this string line)`: parses one normalized line (as produced by `GetPdfPageLines`) into a `TemplateLine`, splitting on the format's own punctuation (`(`, `)`, the en dash, `&`, `:`) to recover the coordinate, member, optional in-law, and optional family dynamic date. Throws `InvalidCastException` if the line's shape doesn't fit one of the valid combinations — an empty line, a member alone, a member with an in-law, or a member with an in-law and a family dynamic date. A malformed line is treated as a bug in the source template, not user input to recover from gracefully: templates are PDFs, generated from a known format, so a line that doesn't fit is a developer-facing failure.

`TemplateReader` (`Drive/Domain`) is what actually drives this end to end. Given a blob name, `ReadTemplateAsync` fetches it via `IFamilyDriveRepository`, confirms its content type is `Application_PDF` — anything else throws `InvalidOperationException`, same as a blob that isn't found — and returns a lazy `IEnumerable<TemplateLine>` backed by a custom `IEnumerator<TemplateLine>`. That enumerator only ever holds one page's worth of normalized lines in memory at a time: `MoveNext()` drains the current page's queue one `TemplateLine` at a time, and once it's empty, fetches, extracts, and normalizes the next page before continuing — so a large multi-page template is never fully parsed up front, and pages are pulled from Azure Blob Storage lazily as the caller actually enumerates. Logging is split by intent: `Information` covers per-read lifecycle (opening a blob, and one summary line on close reporting how many lines/pages it produced), while per-page and per-line detail — including real member names and dates — is `Debug`, so it only reaches a log sink when someone has deliberately raised the level to investigate a specific read.

`TemplateReader` is resolved through Dependency Injection, not constructed directly. `DriveExtensions.AddFamilyDrive(this IHostApplicationBuilder builder)` registers both it and `IFamilyDriveRepository`/`FamilyDrive` into the host's service collection; call it once during startup, after configuration is wired up via `AddConstantStorePipeline`, and any of the four execution contexts — Console, API, Functions, or an NUnit test's own `Host.CreateApplicationBuilder()` — can resolve a ready-to-use `TemplateReader` through constructor injection. `FamilyDrive`'s `IConfiguration` dependency is resolved from the container itself rather than captured from the builder directly, which works uniformly across all four contexts because `IConfiguration` is registered automatically by any `IHostApplicationBuilder` (`HostApplicationBuilder`, `WebApplicationBuilder`, and the Azure Functions isolated-worker builder all guarantee this) before `AddFamilyDrive` ever runs.

# Repository
- `IFamilyDriveRepository`: the contract for the drive — `GetAsync`, `SaveAsync`, and `DeleteAsync`, each keyed by a container-prefixed blob name (see Drive Extensions) and each returning `null` when the target blob doesn't exist. Blob storage only — it has no awareness of `HierarchicalCoordinate` or `TemplateLine`; a template blob's `Content` stream is just bytes to it.
- `FamilyDrive`: the Azure Blob Storage implementation. Authenticates via `DefaultAzureCredential`, so whatever identity runs it — a developer signed in via `az login`, or the ACA-hosted API's managed identity — needs Storage Blob Data Owner/Contributor on this account. It reads its endpoint and container names from configuration (see Key Constants), then resolves the target `BlobClient` by routing on the blob name's prefix and stripping that prefix off before handing the remainder to the Azure SDK, since the SDK's container clients are already scoped to a single container.

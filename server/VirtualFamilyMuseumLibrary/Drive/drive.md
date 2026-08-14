## Family Drive
# Table Of Contents
1. Overview
2. Key Constants
3. Drive Extensions
4. Models
5. Repository

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

# Repository
- `IFamilyDriveRepository`: the contract for the drive — `GetAsync`, `SaveAsync`, and `DeleteAsync`, each keyed by a container-prefixed blob name (see Drive Extensions) and each returning `null` when the target blob doesn't exist.
- `FamilyDrive`: the Azure Blob Storage implementation. Authenticates via `DefaultAzureCredential`, so whatever identity runs it — a developer signed in via `az login`, or the ACA-hosted API's managed identity — needs Storage Blob Data Owner/Contributor on this account. It reads its endpoint and container names from configuration (see Key Constants), then resolves the target `BlobClient` by routing on the blob name's prefix and stripping that prefix off before handing the remainder to the Azure SDK, since the SDK's container clients are already scoped to a single container.

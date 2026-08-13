using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualFamilyMuseumLibrary.Drive.Models
{
    public class FamilyBlobResource
    {
        public required string BlobName
        {
            get;
            init;
        }

        public required string BlobUrl
        {
            get;
            init;
        }

        public required Stream Content
        {
            get;
            init;
        }

        public required FamilyContentTypes ContentType
        {
            get;
            init;
        }
    }
}
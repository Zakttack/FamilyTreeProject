using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualFamilyMuseumLibrary.Configuration.Repositories
{
    public interface ISensitiveConstantRepository
    {
        public Task<string> GetSecretAsync(string name);
    }
}
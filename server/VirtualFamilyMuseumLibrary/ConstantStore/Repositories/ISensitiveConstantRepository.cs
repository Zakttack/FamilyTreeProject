using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualFamilyMuseumLibrary.ConstantStore.Repositories
{
    public interface ISensitiveConstantRepository
    {
        public string GetSecret(string name);
    }
}
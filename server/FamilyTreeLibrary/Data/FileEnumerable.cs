using FamilyTreeLibrary.Data.Enumerators;
using System.Collections;

namespace FamilyTreeLibrary.Data
{
    public class FileEnumerable : IEnumerable<string>
    {
        public IEnumerator<string> GetEnumerator()
        {
            return new FileEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FileEnumerator();
        }
    }
}
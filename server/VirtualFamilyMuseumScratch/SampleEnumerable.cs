using System.Collections;

namespace VirtualFamilyMuseumScratch
{
    public class SampleEnumerable : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            return new SampleEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SampleEnumerator();
        }
    }
}
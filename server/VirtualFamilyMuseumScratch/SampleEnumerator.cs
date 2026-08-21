using System.Collections;

namespace VirtualFamilyMuseumScratch
{
    public class SampleEnumerator : IEnumerator<int>
    {
        private int current = 0;

        public int Current
        {
            get
            {
                Console.WriteLine($"Current is {current}");
                return current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                Console.WriteLine($"Current (from IEnumerator) is {current}");
                return current;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose was called.");
        }

        public bool MoveNext()
        {
            Console.WriteLine("Move Next was called.");
            if (current >= 5)
            {
                return false;
            }
            current++;
            return true;
        }

        public void Reset()
        {
            Console.WriteLine("Reset was called.");
            current = 0;
        }
    }
}
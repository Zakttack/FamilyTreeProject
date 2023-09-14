using FamilyTreeLibrary.Exceptions;
using System.Collections;

namespace FamilyTreeLibrary.OrderingType
{
    public class LetterOrderingType : AbstractOrderingType
    {
        public LetterOrderingType(int key, OrderingTypeOptions option)
        :base(key)
        {
            Enumerator = new LetterEnumerator();
            Type = option == OrderingTypeOptions.UpperCase ? OrderingTypeTypes.CapitalLetter : OrderingTypeTypes.LowerCaseLetter;
        }

        public LetterOrderingType(string value, OrderingTypeOptions option)
        :base(value)
        {
            Enumerator = new LetterEnumerator();
            Type = option == OrderingTypeOptions.UpperCase ? OrderingTypeTypes.CapitalLetter : OrderingTypeTypes.LowerCaseLetter;
        }

        protected override OrderingTypeTypes Type
        {
            get;
        }

        protected override int FindKey(string value)
        {
            string v = value.Trim('.').ToUpper();
            foreach (char c in v)
            {
                if (!char.IsAsciiLetterUpper(c))
                {
                    throw new InvalidValueException(value);
                }
            }
            int key = 1;
            while (FindValue(key) != value)
            {
                key++;
            }
            return key;
        }

        protected override string FindValue(int key)
        {
            if (key < 0)
            {
                throw new InvalidKeyException(key);
            }
            for (int k = 1; k < key; k++)
            {
                Enumerator.Dispose();
            }
            string value = Enumerator.Current;
            switch (Type)
            {
                case OrderingTypeTypes.CapitalLetter: value += "."; break;
                case OrderingTypeTypes.LowerCaseLetter: value = $"{value.ToLower()}."; break;
                default: throw new ArgumentNullException("Ordering Type Options", "A letter is either capitalized or not capitalized.");
            }
            Enumerator.Reset();
            return value;
        }

        private IEnumerator<string> Enumerator
        {
            get;
        }

        private class LetterEnumerator : IEnumerator<string>
        {
            public LetterEnumerator()
            {
                Letters = new List<char>()
                {
                    'A'
                };
            }

            public string Current
            {
                get
                {
                    return new(Letters.ToArray());
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                Increment(Letters[^1]);
            }

            public bool MoveNext()
            {
                return true;
            }

            public void Reset()
            {
                Letters.Clear();
                Letters.Add('A');
            }

            private IList<char> Letters
            {
                get;
            }

            private void Increment(int position)
            {
                if (position < 0)
                {
                    Letters.Insert(0, 'A');
                }
                else if (Letters[position] == 'Z')
                {
                    Letters[position] = 'A';
                    Increment(position - 1);
                }
                else
                {
                    int asciiValue = Letters[position] + 1;
                    Letters[position] = (char)asciiValue;
                }
            }
        }
    }
}
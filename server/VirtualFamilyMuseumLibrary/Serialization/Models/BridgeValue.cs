using System.Collections;
using System.Text.Json;
namespace VirtualFamilyMuseumLibrary.Serialization.Models
{
    public readonly struct BridgeValue : IEquatable<BridgeValue>
    {
        private readonly object? value;

        public BridgeValue()
        {
            value = null;
        }

        public BridgeValue(IDictionary<string,BridgeValue> value)
        {
            this.value = new Dictionary<string,BridgeValue>(value);
        }

        public BridgeValue(IList<BridgeValue> value)
        {
            this.value = new List<BridgeValue>(value);
        }
        
        private BridgeValue(string value)
        {
            this.value = value;
        }

        private BridgeValue(Number value)
        {
            this.value = value;
        }

        private BridgeValue(bool value)
        {
            this.value = value;
        }

        public readonly bool IsNull
        {
            get => value is null;
        }

        public readonly IDictionary<string,BridgeValue> AsObject
        {
            get
            {
                if (value is null)
                {
                    throw new InvalidCastException("The value doesn't exist.");
                }
                else if (value is IDictionary<string,BridgeValue> output)
                {
                    return new Dictionary<string,BridgeValue>(output);
                }
                throw new InvalidCastException("The value isn't of type Object.");
            }
        }

        public readonly IList<BridgeValue> AsArray
        {
            get
            {
                if (value is null)
                {
                    throw new InvalidCastException("The value doesn't exist.");
                }
                else if (value is IList<BridgeValue> output)
                {
                    return [.. output];
                }
                throw new InvalidCastException("The value isn't of type Array.");
            }
        }

        public readonly bool Equals(BridgeValue other)
        {
            if (value is null && other.value is null)
            {
                return true;
            }
            else if (value is null && other.value is not null)
            {
                return false;
            }
            else if (value is not null && other.value is null)
            {
                return false;
            }
            else if (value is string a && other.value is string b)
            {
                return a == b;
            }
            else if (value is Number c && other.value is Number d)
            {
                return c == d;
            }
            else if (value is bool e && other.value is bool f)
            {
                return !(e ^ f);
            }
            else if (value is IDictionary<string,BridgeValue> obj1 && other.value is IDictionary<string,BridgeValue> obj2)
            {
                if (obj1.Count != obj2.Count)
                {
                    return false;
                }
                foreach (string key in obj1.Keys)
                {
                    if (!obj2.TryGetValue(key, out BridgeValue output))
                    {
                        return false;
                    }
                    else if (!obj1[key].Equals(output))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (value is IList<BridgeValue> array1 && other.value is IList<BridgeValue> array2)
            {
                return array1.SequenceEqual(array2);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            return obj is BridgeValue v && Equals(v);
        }

        public override int GetHashCode()
        {
            if (value is null)
            {
                return 0;
            }
            else if (value is string text)
            {
                return text.GetHashCode();
            }
            else if (value is Number number)
            {
                return number.GetHashCode();
            }
            else if (value is bool output)
            {
                return output.GetHashCode();
            }
            else if (value is IDictionary<string,BridgeValue> obj)
            {
                IEnumerable<int> codes = obj.Select((pair) => pair.Key.GetHashCode() + (2 * pair.Value.GetHashCode())).Order();
                IEnumerator<int> enumerator = new PrimeNumberEnumerator();
                int result = 0;
                foreach (int code in codes)
                {
                    result += enumerator.Current * code;
                    enumerator.MoveNext();
                }
                return result;
            }
            else if (value is IList<BridgeValue> array)
            {
                IEnumerator<int> enumerator = new PrimeNumberEnumerator();
                int result = 0;
                foreach (BridgeValue element in array)
                {
                    result += enumerator.Current * element.GetHashCode();
                    enumerator.MoveNext();
                }
                return result;
            }
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(value, SerializationExtensions.GetOptions(true));
        }

        public bool TryAsString(out string? text)
        {
            text = (value is not null && value is string output) ? output : null;
            return text is not null;
        }

        public bool TryAsNumber(out Number? number)
        {
            number = (value is not null && value is Number output) ? output : null;
            return number is not null;
        }

        public bool TryAsBool(out bool? result)
        {
            result = (value is not null && value is bool output) ? output : null;
            return result is not null;
        }

        public bool TryAsObject(out IDictionary<string,BridgeValue>? obj)
        {
            obj = (value is not null && value is IDictionary<string,BridgeValue> output) ? new Dictionary<string,BridgeValue>(output) : null;
            return obj is not null;
        }

        public bool TryAsArray(out IList<BridgeValue>? array)
        {
            array = (value is not null && value is IList<BridgeValue> output) ? [.. output] : null;
            return array is not null;
        }

        public static implicit operator BridgeValue(string input)
        {
            return new(input);
        }

        public static explicit operator string(BridgeValue input)
        {
            return input.AsString;
        }

        public static implicit operator BridgeValue(Number input)
        {
            return new(input);
        }

        public static explicit operator Number(BridgeValue input)
        {
            return input.AsNumber;
        }

        public static implicit operator BridgeValue(bool input)
        {
            return new(input);
        }

        public static explicit operator bool(BridgeValue input)
        {
            return input.AsBool;
        }

        public static bool operator==(BridgeValue a, BridgeValue b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(BridgeValue a, BridgeValue b)
        {
            return !a.Equals(b);
        }

        private readonly string AsString
        {
            get
            {
                if (value is null)
                {
                    throw new InvalidCastException("The value doesn't exist.");
                }
                else if (value is string output)
                {
                    return output;
                }
                throw new InvalidCastException("The value isn't of type string.");
            }
        }

        private readonly Number AsNumber
        {
            get
            {
                if (value is null)
                {
                    throw new InvalidCastException("The value doesn't exist.");
                }
                else if (value is Number output)
                {
                    return output;
                }
                throw new InvalidCastException("The value isn't of type Number.");
            }
        }

        private readonly bool AsBool
        {
            get
            {
                if (value is null)
                {
                    throw new InvalidCastException("The value doesn't exist.");
                }
                else if (value is bool output)
                {
                    return output;
                }
                throw new InvalidCastException("The value isn't of type Bool.");
            }
        }

        private class PrimeNumberEnumerator : IEnumerator<int>
        {
            private int current;

            public PrimeNumberEnumerator()
            {
                Reset();
            }

            public int Current
            {
                get => current;
            }

            object IEnumerator.Current
            {
                get => current;
            }

            public void Dispose()
            {
                Reset();
            }

            public bool MoveNext()
            {
                do
                {
                    current++;
                } while (!IsPrime(current));
                return true;
            }

            public void Reset()
            {
                current = 1;
            }

            private static bool IsPrime(int n)
            {
                if (n < 2) return false;
                if (n == 2) return true;
                if (n % 2 == 0) return false;
                for (int i = 3; (long)Math.Pow(i, 2) <= n; i += 2)
                {
                    if (n % i == 0) return false;
                }
                return true;
            }
        }
    }
}
namespace VirtualFamilyMuseumLibrary.Serialization.Models
{
    public struct Number(double value) : IComparable<Number>, IEquatable<Number>
    {
        public readonly int CompareTo(Number other)
        {
            return value.CompareTo(other.AsDouble);
        }

        public readonly bool Equals(Number other)
        {
            return CompareTo(other) == 0;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Number other && Equals(other);
        }

        public readonly override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public readonly override string ToString()
        {
            if (value >= long.MinValue && value <= long.MaxValue)
            {
                string representation = value.ToString();
                int dotIndex = representation.IndexOf('.');
                if (dotIndex > -1)
                {
                    string decimalPart = representation[(dotIndex + 1)..];
                    int dec = Convert.ToInt32(decimalPart);
                    if (dec == 0)
                    {
                        return representation[..(representation.Length - dotIndex - 1)];
                    }
                }
            }
            return value.ToString();
        }

        public readonly bool TryGetDouble(out double output)
        {
            output = value;
            return true;
        }

        public readonly bool TryGetInt(out int output)
        {
            if (value > int.MaxValue || value < int.MinValue || value != Math.Truncate(value))
            {
                output = 0;
                return false;
            }
            output = (int)value;
            return true;
        }

        public readonly bool TryGetLong(out long output)
        {
            if (value > long.MaxValue || value < long.MinValue || value != Math.Truncate(value))
            {
                output = 0;
                return false;
            }
            output = (long)value;
            return true;
        }

        public static implicit operator Number(double value)
        {
            return new Number(value);
        }

        public static explicit operator double(Number value)
        {
            return value.AsDouble;
        }

        public static implicit operator Number(long value)
        {
            return new Number(value);
        }

        public static explicit operator long(Number value)
        {
            return value.AsLong;
        }

        public static explicit operator int(Number value)
        {
            return value.AsInt;
        }

        public static bool operator==(Number a, Number b)
        {
            return a.Equals(b);
        }

        public static bool operator!=(Number a, Number b)
        {
            return !a.Equals(b);
        }

        public static bool operator<(Number a, Number b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator>(Number a, Number b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator<=(Number a, Number b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator>=(Number a, Number b)
        {
            return a.CompareTo(b) >= 0;
        }

        private readonly double AsDouble
        {
            get => value;
        }

        private readonly long AsLong
        {
            get
            {
                if (value > long.MaxValue || value < long.MinValue)
                {
                    throw new InvalidCastException("The Number must within range of [2^63 - 1,-2^63].");
                }
                else if (value != Math.Truncate(value))
                {
                    throw new InvalidCastException("The value must be whole.");
                }
                return (long)value;
            }
        }

        private readonly int AsInt
        {
            get
            {
                if (value > int.MaxValue || value < int.MinValue)
                {
                    throw new InvalidCastException("The Number must within range of [2^31 - 1,-2^31].");
                }
                else if (value != Math.Truncate(value))
                {
                    throw new InvalidCastException("The value must be whole.");
                }
                return (int)value;
            }
        }
    }
}
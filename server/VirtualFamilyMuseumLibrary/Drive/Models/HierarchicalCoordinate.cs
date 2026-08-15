using System.Text;

namespace VirtualFamilyMuseumLibrary.Drive.Models
{
    public readonly struct HierarchicalCoordinate(int[] coords) : IComparable<HierarchicalCoordinate>, IEquatable<HierarchicalCoordinate>
    {
        private readonly int[] coordinates = [.. coords];
        public HierarchicalCoordinate? Parent
        {
            get
            {
                if (coordinates.Length == 0)
                {
                    return null;
                }
                return new HierarchicalCoordinate(coordinates[..^1]);
            }
        }
        public HierarchicalCoordinate Child
        {
            get
            {
                int[] childCoordinates = new int[coordinates.Length + 1];
                Array.Copy(coordinates, childCoordinates, coordinates.Length);
                childCoordinates[^1] = 1;
                return new HierarchicalCoordinate([.. childCoordinates]);
            }
        }

        public HierarchicalCoordinate? NextSibling
        {
            get
            {
                if (coordinates.Length == 0)
                {
                    return null;
                }
                int[] siblingCoordinates = [.. coordinates];
                siblingCoordinates[^1] = coordinates[^1] + 1;
                return new HierarchicalCoordinate([.. siblingCoordinates]);
            }
        }
        public int CompareTo(HierarchicalCoordinate other)
        {
            if (coordinates.Length == 0 && other.coordinates.Length == 0)
            {
                return 0;
            }
            else if (coordinates.Length == 0 && other.coordinates.Length > 0)
            {
                return -1;
            }
            else if (coordinates.Length > 0 && other.coordinates.Length == 0)
            {
                return 1;
            }
            for (int i = 0; i < coordinates.Length && i < other.coordinates.Length; i++)
            {
                if (coordinates[i] < other.coordinates[i])
                {
                    return -1;
                }
                else if (coordinates[i] > other.coordinates[i])
                {
                    return 1;
                }
            }
            return coordinates.Length - other.coordinates.Length;
        }

        public bool Equals(HierarchicalCoordinate other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is HierarchicalCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            foreach (int coord in coordinates)
            {
                hash.Add(coord);
            }
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            if (coordinates.Length == 0)
            {
                return "";
            }
            StringBuilder builder = new(string.Join('.', coordinates));
            builder.Append(')');
            return builder.ToString();
        }

        public static bool operator==(HierarchicalCoordinate? a, HierarchicalCoordinate? b)
        {
            if (!a.HasValue && !b.HasValue)
            {
                return true;
            }
            else if (!a.HasValue && b.HasValue)
            {
                return false;
            }
            else if (a.HasValue && !b.HasValue)
            {
                return false;
            }
            return a.HasValue && b.HasValue && a.Value.Equals(b.Value);
        }

        public static bool operator!=(HierarchicalCoordinate? a, HierarchicalCoordinate? b)
        {
            if (!a.HasValue && !b.HasValue)
            {
                return false;
            }
            else if (!a.HasValue && b.HasValue)
            {
                return true;
            }
            else if (a.HasValue && !b.HasValue)
            {
                return true;
            }
            return !(a.HasValue && b.HasValue && a.Value.Equals(b.Value));
        }

        public static bool operator<(HierarchicalCoordinate? a, HierarchicalCoordinate? b)
        {
            if (!a.HasValue && !b.HasValue)
            {
                return false;
            }
            else if (!a.HasValue && b.HasValue)
            {
                return true;
            }
            else if (a.HasValue && !b.HasValue)
            {
                return false;
            }
            return a.HasValue && b.HasValue && a.Value.CompareTo(b.Value) < 0;
        }

        public static bool operator>(HierarchicalCoordinate? a, HierarchicalCoordinate? b)
        {
            if (!a.HasValue && !b.HasValue)
            {
                return false;
            }
            else if (!a.HasValue && b.HasValue)
            {
                return false;
            }
            else if (a.HasValue && !b.HasValue)
            {
                return true;
            }
            return a.HasValue && b.HasValue && a.Value.CompareTo(b.Value) > 0;
        }

        public static bool operator<=(HierarchicalCoordinate? a, HierarchicalCoordinate? b)
        {
            if (!a.HasValue && !b.HasValue)
            {
                return true;
            }
            else if (!a.HasValue && b.HasValue)
            {
                return true;
            }
            else if (a.HasValue && !b.HasValue)
            {
                return false;
            }
            return a.HasValue && b.HasValue && a.Value.CompareTo(b.Value) <= 0;
        }

        public static bool operator>=(HierarchicalCoordinate? a, HierarchicalCoordinate? b)
        {
            if (!a.HasValue && !b.HasValue)
            {
                return true;
            }
            else if (!a.HasValue && b.HasValue)
            {
                return false;
            }
            else if (a.HasValue && !b.HasValue)
            {
                return true;
            }
            return a.HasValue && b.HasValue && a.Value.CompareTo(b.Value) >= 0;
        }
    }
}
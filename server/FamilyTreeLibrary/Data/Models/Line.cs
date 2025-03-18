using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.Models
{
    public readonly struct Line(HierarchialCoordinate coordinate, Person member, Person? inLaw = null, Partnership? partnership = null) : ICopyable<Line>, IEquatable<Line>
    {
        public HierarchialCoordinate Coordinate
        {
            get => coordinate;
        }
        public Person Member
        {
            get => member;
        }

        public Person? InLaw
        {
            get => inLaw;
        }

        public Partnership? Partnership
        {
            get => partnership;
        }

        public Line Copy()
        {
            return new Line(Coordinate, Member, InLaw, Partnership);
        }

        public bool Equals(Line other)
        {
            return Coordinate == other.Coordinate && Member == other.Member && InLaw == other.InLaw && partnership == other.Partnership;
        }

        public override bool Equals(object? obj)
        {
            return obj is Line line && Equals(line);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Coordinate, Member, InLaw, Partnership);
        }

        public override string ToString()
        {
            string representation = $"{Coordinate} {Member}";
            if (InLaw is not null)
            {
                representation += $" & {InLaw}";
            }
            if (Partnership is not null)
            {
                representation += $": {Partnership}";
            }
            return representation;
        }

        public static bool operator==(Line? a, Line? b)
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
            return a is not null && b is not null && a.Value.Equals(b.Value);
        }

        public static bool operator!=(Line? a, Line? b)
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
            return a is null || b is null || !a.Value.Equals(b.Value);
        }
    }
}
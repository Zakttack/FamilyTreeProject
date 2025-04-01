using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.Models
{
    public readonly struct Line(HierarchialCoordinate coordinate, Person member, Person? inLaw = null, FamilyDynamic? familyDynamic = null) : ICopyable<Line>, IEquatable<Line>
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

        public FamilyDynamic? FamilyDynamic
        {
            get => familyDynamic;
        }

        public Line Copy()
        {
            return new Line(Coordinate, Member, InLaw, FamilyDynamic);
        }

        public bool Equals(Line other)
        {
            return Coordinate == other.Coordinate && Member == other.Member && InLaw == other.InLaw && familyDynamic == other.FamilyDynamic;
        }

        public override bool Equals(object? obj)
        {
            return obj is Line line && Equals(line);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Coordinate, Member, InLaw, FamilyDynamic);
        }

        public override string ToString()
        {
            string representation = $"{Coordinate} {Member}";
            if (InLaw is not null)
            {
                representation += $" & {InLaw}";
            }
            if (FamilyDynamic is not null)
            {
                representation += $": {FamilyDynamic}";
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
using System.Text;
using VirtualFamilyMuseumLibrary.Models;

namespace VirtualFamilyMuseumLibrary.Drive.Models
{
    // Assumes upstream validation already enforces these invariants — this model does not check them itself:
    // - MemberDeceasedDate is only set when MemberBirthDate is set.
    // - InLawBirthDate/InLawDeceasedDate/FamilyDynamicStartDate are only set when InLawBirthName is set.
    public class TemplateLine : IComparable<TemplateLine>, IEquatable<TemplateLine>
    {
        public required HierarchicalCoordinate Coordinate {get; init;}
        public required string MemberBirthName {get; init;}
        public FamilyDate? MemberBirthDate {get; init;}
        public FamilyDate? MemberDeceasedDate {get; init;}
        public string? InLawBirthName {get; init;}
        public FamilyDate? InLawBirthDate {get; init;}
        public FamilyDate? InLawDeceasedDate {get; init;}
        public FamilyDate? FamilyDynamicStartDate {get; init;}

        public int CompareTo(TemplateLine? other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (Coordinate < other.Coordinate)
            {
                return -1;
            }
            else if (Coordinate > other.Coordinate)
            {
                return 1;
            }
            int memberBirthNameCompare = MemberBirthName.CompareTo(other.MemberBirthName);
            if (memberBirthNameCompare != 0)
            {
                return memberBirthNameCompare;
            }
            else if (MemberBirthDate < other.MemberBirthDate)
            {
                return -1;
            }
            else if (MemberBirthDate > other.MemberBirthDate)
            {
                return 1;
            }
            else if (MemberDeceasedDate < other.MemberDeceasedDate)
            {
                return -1;
            }
            else if (MemberDeceasedDate > other.MemberDeceasedDate)
            {
                return 1;
            }
            else if (FamilyDynamicStartDate < other.FamilyDynamicStartDate)
            {
                return -1;
            }
            else if (FamilyDynamicStartDate > other.FamilyDynamicStartDate)
            {
                return 1;
            }
            else if (InLawBirthName is null && other.InLawBirthName is not null)
            {
                return -1;
            }
            else if (InLawBirthName is not null && other.InLawBirthName is null)
            {
                return 1;
            }
            else if (InLawBirthName is not null && other.InLawBirthName is not null)
            {
                int inLawBirthNameCompare = InLawBirthName.CompareTo(other.InLawBirthName);
                if (inLawBirthNameCompare != 0)
                {
                    return inLawBirthNameCompare;
                }
            }
            if (InLawBirthDate < other.InLawBirthDate)
            {
                return -1;
            }
            else if (InLawBirthDate > other.InLawBirthDate)
            {
                return 1;
            }
            else if (InLawDeceasedDate < other.InLawDeceasedDate)
            {
                return -1;
            }
            else if (InLawDeceasedDate > other.InLawDeceasedDate)
            {
                return 1;
            }
            return 0;
        }

        public bool Equals(TemplateLine? other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is TemplateLine other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Coordinate, MemberBirthName, MemberBirthDate, MemberDeceasedDate, InLawBirthName, InLawBirthDate, InLawDeceasedDate, FamilyDynamicStartDate);
        }

        public override string ToString()
        {
            StringBuilder builder = new($"{Coordinate} ");
            builder.Append(MemberBirthName);
            if (!MemberBirthDate.HasValue)
            {
                builder.Append($" ({Extensions.EN_DASH})");
            }
            else if (MemberBirthDate.HasValue && !MemberDeceasedDate.HasValue)
            {
                builder.Append($" ({MemberBirthDate.Value} {Extensions.EN_DASH} Present)");
            }
            else if (MemberBirthDate.HasValue && MemberDeceasedDate.HasValue)
            {
                builder.Append($" ({MemberBirthDate.Value} {Extensions.EN_DASH} {MemberDeceasedDate.Value})");
            }
            if (InLawBirthName is not null)
            {
                builder.Append($" & {InLawBirthName}");
                if (!InLawBirthDate.HasValue)
                {
                    builder.Append($" ({Extensions.EN_DASH})");
                }
                else if (InLawBirthDate.HasValue && !InLawDeceasedDate.HasValue)
                {
                    builder.Append($" ({InLawBirthDate.Value} {Extensions.EN_DASH} Present)");
                }
                else if (InLawBirthDate.HasValue && InLawDeceasedDate.HasValue)
                {
                    builder.Append($" ({InLawBirthDate.Value} {Extensions.EN_DASH} {InLawDeceasedDate.Value})");
                }
            }
            if (FamilyDynamicStartDate.HasValue)
            {
                builder.Append($": {FamilyDynamicStartDate.Value}");
            }
            return builder.ToString().Trim();
        }

        public static bool operator==(TemplateLine? a, TemplateLine? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            return a is not null && a.Equals(b);
        }

        public static bool operator!=(TemplateLine? a, TemplateLine? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            return !(a is not null && a.Equals(b));
        }

        public static bool operator<(TemplateLine? a, TemplateLine? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            return a is not null && a.CompareTo(b) < 0;
        }

        public static bool operator>(TemplateLine? a, TemplateLine? b)
        {
            return a is not null && a.CompareTo(b) > 0;
        }

        public static bool operator<=(TemplateLine? a, TemplateLine? b)
        {
            return a is null || a.CompareTo(b) <= 0;
        }

        public static bool operator>=(TemplateLine? a, TemplateLine? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            return a is not null && a.CompareTo(b) >= 0;
        }
    }
}
using FamilyTreeAPI.Models;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;

namespace FamilyTreeAPI
{
    public static class APIUtils
    {
        public static Family DeserializeFamilyElement(FamilyElement element)
        {
            Person member = new(element.Member.Name, new FamilyTreeDate(element.Member.BirthDate), new FamilyTreeDate(element.Member.DeceasedDate));
            Person inLaw = element.InLaw is null ? null : new(element.InLaw.Name, new FamilyTreeDate(element.InLaw.BirthDate), new FamilyTreeDate(element.InLaw.DeceasedDate));
            FamilyTreeDate marriageDate = new(element.MarriageDate);
            return new(member, inLaw, marriageDate);
        }

        public static Person DeserializePersonElement(PersonElement element)
        {
            if (element is null || (element.Name is null && element.BirthDate is null && element.DeceasedDate is null))
            {
                throw new PersonNotFoundException("The person doesn't exist.");
            }
            return new(element.Name, new FamilyTreeDate(element.BirthDate), new FamilyTreeDate(element.DeceasedDate));
        }
        
        public static ExceptionResponse SerializeErrorResponse(Exception ex)
        {
            return new ExceptionResponse
            {
                Name = ex.GetType().Name,
                Message = ex.Message
            };
        }

        public static FamilyElement SerializeFamily(Family family)
        {
            return new FamilyElement
            {
                Member = new PersonElement
                {
                    Name = family.Member.Name,
                    BirthDate = family.Member.BirthDate.ToString(),
                    DeceasedDate = family.Member.DeceasedDate.ToString()
                },
                InLaw = family.InLaw is null ? null : new PersonElement
                {
                    Name = family.InLaw.Name,
                    BirthDate = family.InLaw.BirthDate.ToString(),
                    DeceasedDate = family.InLaw.DeceasedDate.ToString()
                },
                MarriageDate = family.MarriageDate.ToString()
            };
        }
    }
}
using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FamilyTreeAPI
{
    public static class APIUtils
    {
        public static Family DeserializeFamilyElement(FamilyElement element)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"This family element: \"{element}\" is being deserialized.");
                Person member = DeserializePersonElement(element.Member);
                Person inLaw = DeserializePersonElement(element.InLaw);
                FamilyTreeDate marriageDate = new(element.MarriageDate);
                return new(member, inLaw, marriageDate);
            }
            catch (ClientException ex)
            {
                throw ex;
            }
            catch (FormatException ex)
            {
                throw new ClientBadRequestException(ex.Message, ex);
            }
            catch (NullReferenceException)
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Warning, "The family doesn't exist.");
                return null;
            }
            catch (Exception ex)
            {
                throw new ServerException(ex);
            }
        }

        public static Person DeserializePersonElement(PersonElement element)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"This person element: \"{element}\" is being deserialized.");
                return new(element.Name, new FamilyTreeDate(element.BirthDate), new FamilyTreeDate(element.DeceasedDate));
            }
            catch (ClientException ex)
            {
                throw ex;
            }
            catch (FormatException ex)
            {
                throw new ClientBadRequestException(ex.Message, ex);
            }
            catch (NullReferenceException)
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Warning, "The person doesn't exist.");
                return null;
            }
            catch (Exception ex)
            {
                throw new ServerException(ex);
            }
        }
        
        public static ObjectResult SerializeErrorResponse(ControllerBase controller, ClientException ex)
        {
            MessageResponse response = new()
            {
                Message = ex.ToString(),
                IsSuccess = false
            };
            FamilyTreeUtils.LogMessage(LoggingLevels.Error, ex.ToString());
            return ex.StatusCode switch
            {
                HttpStatusCode.BadRequest => controller.BadRequest(response),
                HttpStatusCode.NotFound => controller.NotFound(response),
                _ => controller.Unauthorized(response),
            };
        }

        public static OkObjectResult SerializeFamily(ControllerBase controller, Family family)
        {
            FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The family: \"{family}\" is being serialized.");
            FamilyElement response = GetFamilyElement(family);
            return controller.Ok(response);
        }

        public static OkObjectResult SerializeFamilies(ControllerBase controller, string orderOption, IEnumerable<Family> families)
        {
            FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The families are being serialized in {orderOption} order.");
            return controller.Ok(families.Select(GetFamilyElement));
        }

        public static ObjectResult SerializeFatalResponse(ControllerBase controller, ServerException ex)
        {
            FamilyTreeUtils.LogMessage(LoggingLevels.Fatal, ex.ToString());
            MessageResponse response = new()
            {
                Message = ex.ToString(),
                IsSuccess = false
            };
            return controller.StatusCode(500, response);
        }

        private static FamilyElement GetFamilyElement(Family family)
        {
            return new()
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
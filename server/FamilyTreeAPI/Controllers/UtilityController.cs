using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : ControllerBase
    {
        [HttpGet("get-file-paths/{fileName}")]
        public IActionResult GetFilePaths([FromRoute] string fileName)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Searching for file paths consisting of a file name of {fileName}");
                IEnumerable<string> filePaths = FamilyTreeUtils.GetFilePathsOf(fileName);
                if (!filePaths.Any())
                {
                    throw new ClientNotFoundException($"{fileName} doesn't exist in your directory.");
                }
                return Ok(filePaths.Select((filePath) =>
                {
                    return new FileElement()
                    {
                        FilePath = filePath
                    };
                }));
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeErrorResponse(this, new ClientBadRequestException("No file name has been specified.", ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeFatalResponse(this, new ServerException(ex));
            }
        }

        [HttpPost("representation-to-element")]
        public IActionResult RepresentationToElement([FromBody] FamilyRepresentationElement representationElement)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representationElement.Representation}");
                Family family = new(representationElement.Representation);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Family: {family}");
                return APIUtils.SerializeFamily(this, family);
            }
            catch (FormatException ex)
            {
                return APIUtils.SerializeErrorResponse(this, new ClientBadRequestException(ex.Message, ex));
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeErrorResponse(this, new ClientNotFoundException(ex.Message, ex));
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }

        [HttpPost("element-to-representation")]
        public IActionResult ElementToRepresenation([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Family: {family}");
                FamilyRepresentationElement representation = new()
                {
                    Representation = family.ToString()
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representation.Representation}");
                return Ok(representation);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeErrorResponse(this, new ClientNotFoundException(ex.Message, ex));
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }

        [HttpPost("person-element-to-representation")]
        public IActionResult PersonElementToRepresentation([FromBody] PersonElement element)
        {
            try
            {
                Person person = APIUtils.DeserializePersonElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Person: {person}");
                FamilyRepresentationElement representation = new()
                {
                    Representation = person.ToString()
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representation.Representation}");
                return Ok(representation);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeErrorResponse(this, new ClientNotFoundException(ex.Message, ex));
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }
    }
}
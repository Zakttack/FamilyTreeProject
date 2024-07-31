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
        [HttpPost("representation-to-family-element")]
        public IActionResult RepresentationToFamilyElement([FromBody] RepresentationElement representationElement)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representationElement.Representation}");
                Family family = new(representationElement.Representation);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Family: {family}");
                return Ok(APIUtils.SerializeFamily(family));
            }
            catch (FormatException ex)
            {
                return APIUtils.SerializeAsClientError(new ClientBadRequestException(ex.Message, ex));
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClientError(new ClientNotFoundException(ex.Message, ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("family-element-to-representation")]
        public IActionResult FamilyElementToRepresentation([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Family: {family}");
                RepresentationElement representation = new()
                {
                    Representation = family.ToString()
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representation.Representation}");
                return Ok(representation);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClientError(ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClientError(new ClientNotFoundException(ex.Message, ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpGet("get-client-family-name")]
        public IActionResult GetClientFamilyName()
        {
            try
            {
                return APIUtils.ClientFamilyName is null ? Ok(new ClientFamilyNameElement() {FamilyName = APIUtils.STRING_DEFAULT}) : Ok(APIUtils.ClientFamilyName);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpGet("get-client-family-tree")]
        public IActionResult GetClientFamilyTree()
        {
            try
            {
                return APIUtils.ClientFamilyTree is null ? Ok(new ClientFamilyTreeElement()) : Ok(APIUtils.ClientFamilyTree);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpGet("get-client-page-title")]
        public IActionResult GetClientPageTitle()
        {
            try
            {
                return APIUtils.ClientPageTitle is null ? Ok(new ClientPageTitleElement(){Title = APIUtils.STRING_DEFAULT}) : Ok(APIUtils.ClientPageTitle);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpGet("get-client-selected-family")]
        public IActionResult GetClientSelectedFamily()
        {
            try
            {
                return APIUtils.ClientSelectedFamily is null ? Ok(new ClientSelectedFamilyElement() {SelectedFamily = APIUtils.FamilyDefault}) : Ok(APIUtils.ClientSelectedFamily);
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPost("person-element-to-representation")]
        public IActionResult PersonElementToRepresentation([FromBody] PersonElement element)
        {
            try
            {
                Person person = APIUtils.DeserializePersonElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Person: {person}");
                RepresentationElement representation = new()
                {
                    Representation = person.ToString()
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Representation: {representation.Representation}");
                return Ok(representation);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeAsClientError(ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeAsClientError(new ClientNotFoundException(ex.Message, ex));
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPut("set-client-family-name")]
        public IActionResult SetClientFamilyName([FromBody] ClientFamilyNameElement clientFamilyName)
        {
            try
            {
                APIUtils.ClientFamilyName = clientFamilyName;
                FamilyTreeUtils.LogMessage(LoggingLevels.Debug, "The client family name has been updated.");
                return Ok(new MessageResponse() {Message = "The client family name has been updated.", IsSuccess = true});
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }

        }

        [HttpPut("set-client-family-tree")]
        public IActionResult SetClientFamilyTree([FromBody] ClientFamilyTreeElement clientFamilyTree)
        {
            try
            {
                APIUtils.ClientFamilyTree = clientFamilyTree;
                FamilyTreeUtils.LogMessage(LoggingLevels.Debug, "The client family tree has been updated.");
                return Ok(new MessageResponse(){Message = "The client family tree has been updated.", IsSuccess = true});
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPut("set-client-page-title")]
        public IActionResult SetClientPageTitle([FromBody] ClientPageTitleElement clientPageTitle)
        {
            try
            {
                APIUtils.ClientPageTitle = clientPageTitle;
                FamilyTreeUtils.LogMessage(LoggingLevels.Debug, "The client page title has been updated.");
                return Ok(new MessageResponse(){Message = "The client page title has been updated.", IsSuccess = true});
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }

        [HttpPut("set-client-selected-family")]
        public IActionResult SetClientSelectedFamily([FromBody] ClientSelectedFamilyElement clientSelectedFamily)
        {
            try
            {
                APIUtils.ClientSelectedFamily = clientSelectedFamily;
                FamilyTreeUtils.LogMessage(LoggingLevels.Debug, "The client selected family has been updated.");
                return Ok(new MessageResponse(){Message = "The client selected family has been updated.", IsSuccess = true});
            }
            catch (Exception ex)
            {
                return APIUtils.SerializeAsServerError(ex);
            }
        }
    }
}
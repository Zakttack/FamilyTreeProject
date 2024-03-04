using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FamilyTreeController : ControllerBase
    {

        [HttpGet("{familyName}/getfamilies/{orderOption}")]
        public IActionResult GetFamilies([FromRoute]string familyName,[FromRoute]string orderOption)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, "Based on the family name, the tree is being sorted in a certain order.");
                bool familyNameIsProvided = familyName is not null && familyName != "";
                string message = $"Family Name: " + (familyNameIsProvided ? familyName : "Unknown") + "; ";
                bool orderOptionIsProvided = orderOption is not null && orderOption != "";
                message += "Order Option: " + (orderOptionIsProvided ? orderOption : "Unkown") + ";";
                LoggingLevels level = familyNameIsProvided && orderOptionIsProvided ? LoggingLevels.Information : LoggingLevels.Warning;
                FamilyTreeUtils.LogMessage(level, message);
                FamilyTreeService service = new(familyName);
                return orderOption switch
                {
                    "parent first then children" => APIUtils.SerializeFamilies(this, orderOption, service.ParentFirstThenChildren),
                    "ascending by name" => APIUtils.SerializeFamilies(this, orderOption, service.AscendingByName),
                    _ => throw new ClientBadRequestException("There is nothing to show if you don't select an ordering option.")
                };
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }
        
        [HttpGet("{familyName}/number-of-generations")]
        public IActionResult GetNumberOfGenerations(string familyName)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, "Counting the number of generations in the tree.");
                bool familyNameIsProvided = familyName is not null && familyName != "";
                string message = $"Family Name: " + (familyNameIsProvided ? familyName : "Unknown");
                LoggingLevels level = familyNameIsProvided ? LoggingLevels.Information : LoggingLevels.Warning;
                FamilyTreeUtils.LogMessage(level, message);
                FamilyTreeService service = new(familyName);
                return Ok(service.NumberOfGenerations);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }

        [HttpGet("{familyName}/number-of-families")]
        public IActionResult GetNumberOfFamilies(string familyName)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, "Counting the number of families in the tree.");
                bool familyNameIsProvided = familyName is not null && familyName != "";
                string message = $"Family Name: " + (familyNameIsProvided ? familyName : "Unknown");
                LoggingLevels level = familyNameIsProvided ? LoggingLevels.Information : LoggingLevels.Warning;
                FamilyTreeUtils.LogMessage(level, message);
                FamilyTreeService service = new(familyName);
                return Ok(service.NumberOfFamilies);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }

        [HttpPatch("{familyName}/report-married")]
        public IActionResult ReportMarried([FromRoute] string familyName, [FromBody] FamilyElement request)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                Person member = APIUtils.DeserializePersonElement(request.Member);
                Person inLaw = APIUtils.DeserializePersonElement(request.InLaw);
                FamilyTreeDate marriageDate = new(request.MarriageDate);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"A marriage between {request.Member.Name} and {request.InLaw.Name} on {request.MarriageDate} is being reported.");
                service.ReportMarried(member, inLaw, marriageDate);
                MessageResponse response = new()
                {
                    Message = $"The marriage between {request.Member.Name} and {request.InLaw.Name} has been applied to the tree.",
                    IsSuccess = true
                };
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, response.Message);
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeErrorResponse(this, new ClientBadRequestException(ex.Message, ex));
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }

        [HttpPost("{familyName}/retrieveParent")]
        public IActionResult RetrieveParent([FromRoute] string familyName, [FromBody] FamilyElement element)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"In the {familyName} family tree, we are retrieving the parent of {family.Member.Name}");
                return APIUtils.SerializeFamily(this, service.RetrieveParentOf(family));
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }

        [HttpPut("{familyName}/revert-tree")]
        public IActionResult RevertTree([FromRoute] string familyName, [FromBody] FileElement request)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {familyName} family tree is being reverted based on the following file path: {request.FilePath}");
                service.RevertTree(request.FilePath);
                MessageResponse response = new()
                {
                    Message = $"{familyName} tree has been reverted successfully.",
                    IsSuccess = true
                };
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }
    }
}
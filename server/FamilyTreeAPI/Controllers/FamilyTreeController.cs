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
        [HttpGet("get-families/{orderOption}")]
        public IActionResult GetFamilies([FromRoute]string orderOption)
        {
            try
            {
                IReadOnlySet<string> orderOptions = new HashSet<string>(){"parent first then children", "ascending by name"};
                LoggingLevels level = orderOptions.Contains(orderOption) ? LoggingLevels.Information : LoggingLevels.Warning;
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {APIUtils.Service.Name} tree is being sorted in {orderOption} order.");
                return orderOption switch
                {
                    "parent first then children" => APIUtils.SerializeFamilies(this, orderOption, APIUtils.Service.ParentFirstThenChildren),
                    "ascending by name" => APIUtils.SerializeFamilies(this, orderOption, APIUtils.Service.AscendingByName),
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
        
        [HttpGet("number-of-generations")]
        public IActionResult NumberOfGenerations()
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Counting the number of generations in the {APIUtils.Service.Name} family tree.");
                int response = APIUtils.Service.NumberOfGenerations;
                FamilyTreeUtils.LogMessage(LoggingLevels.Information,$"The {APIUtils.Service.Name} family tree has {response} generations.");
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

        [HttpGet("number-of-families")]
        public IActionResult NumberOfFamilies()
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"Counting the number of families in the {APIUtils.Service.Name} tree.");
                long response = APIUtils.Service.NumberOfFamilies;
                FamilyTreeUtils.LogMessage(LoggingLevels.Information,$"{response} families are part of the {APIUtils.Service.Name} family tree.");
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

        [HttpGet("initialize-service/{familyName}")]
        public IActionResult InitializeService([FromRoute] string familyName)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The service is being initialized based on the {familyName} Family Tree.");
                APIUtils.Service = new(familyName);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The service has been initialized based on the {familyName} Family Tree.");
                MessageResponse response = new()
                {
                    Message = $"This is the {familyName} Family Tree.",
                    IsSuccess = true
                };
                return Ok(response);
            }
            catch (ClientException ex)
            {
                return APIUtils.SerializeErrorResponse(this, ex);
            }
            catch (NullReferenceException ex)
            {
                return APIUtils.SerializeErrorResponse(this, new ClientBadRequestException("Family name is unknown.", ex));
            }
            catch (ServerException ex)
            {
                return APIUtils.SerializeFatalResponse(this, ex);
            }
        }

        [HttpPatch("report-married")]
        public IActionResult ReportMarried([FromBody] FamilyElement request)
        {
            try
            {
                Person member = APIUtils.DeserializePersonElement(request.Member);
                Person inLaw = APIUtils.DeserializePersonElement(request.InLaw);
                FamilyTreeDate marriageDate = new(request.MarriageDate);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"A marriage between {request.Member.Name} and {request.InLaw.Name} on {request.MarriageDate} is being reported.");
                APIUtils.Service.ReportMarried(member, inLaw, marriageDate);
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

        [HttpPost("retrieve-parent")]
        public IActionResult RetrieveParent([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"In the {APIUtils.Service.Name} family tree, we are retrieving the parent of {family.Member.Name}");
                return APIUtils.SerializeFamily(this, APIUtils.Service.RetrieveParentOf(family));
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

        [HttpPut("revert-tree")]
        public IActionResult RevertTree([FromBody] FileElement request)
        {
            try
            {
                FamilyTreeUtils.LogMessage(LoggingLevels.Information, $"The {APIUtils.Service.Name} family tree is being reverted based on the following file path: {request.FilePath}");
                APIUtils.Service.RevertTree(request.FilePath);
                MessageResponse response = new()
                {
                    Message = $"{APIUtils.Service.Name} tree has been reverted successfully.",
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
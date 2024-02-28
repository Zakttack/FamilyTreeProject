using System.Linq;
using System.Runtime.CompilerServices;
using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Service;
using iText.StyledXmlParser.Css.Util;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FamilyTreeController : ControllerBase
    {

        [HttpGet("{familyName}/getfamilies/{orderOption}")]
        public IActionResult GetFamilies(string familyName, string orderOption)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                return orderOption switch
                {
                    "parent first then children" => Ok(service.ParentFirstThenChildren.Select(APIUtils.SerializeFamily)),
                    "ascending by name" => Ok(service.AscendingByName.Select(APIUtils.SerializeFamily)),
                    _ => throw new ArgumentException("There is nothing to show if you don't select an ordering option.")
                };
            }
            catch (ArgumentException ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return BadRequest(APIUtils.SerializeErrorResponse(ex));
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }
        
        [HttpGet("{familyName}/numberofgenerations")]
        public IActionResult GetNumberOfGenerations(string familyName)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                return Ok(service.NumberOfGenerations);
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }

        [HttpGet("{familyName}/numberoffamilies")]
        public IActionResult GetNumberOfFamilies(string familyName)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                return Ok(service.NumberOfFamilies);
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }

        [HttpPatch("{familyName}/report-married")]
        public IActionResult ReportMarried([FromRoute] string familyName, [FromBody] FamilyElement request)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                Person member = new(request.Member.Name, new FamilyTreeDate(request.Member.BirthDate), new FamilyTreeDate(request.Member.DeceasedDate));
                Person inLaw = new(request.InLaw.Name, new FamilyTreeDate(request.InLaw.BirthDate), new FamilyTreeDate(request.InLaw.DeceasedDate));
                FamilyTreeDate marriageDate = new(request.MarriageDate);
                service.ReportMarried(member, inLaw, marriageDate);
                SuccessResponse response = new()
                {
                    Message = $"The marriage between {request.Member.Name} and {request.InLaw.Name} has been applied to the tree."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                ExceptionResponse response = APIUtils.SerializeErrorResponse(ex);
                return ex is ArgumentException || ex is FormatException ? BadRequest(response) : StatusCode(500, response);
            }
        }

        [HttpPost("{familyName}/retrieveParent")]
        public IActionResult RetrieveParent([FromRoute] string familyName, [FromBody] FamilyElement element)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyElement result = APIUtils.SerializeFamily(service.RetrieveParentOf(family));
                return Ok(result);
            }
            catch (FamilyNotFoundException ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return NotFound(APIUtils.SerializeErrorResponse(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }

        [HttpPut("{familyName}/revert-tree")]
        public IActionResult RevertTree([FromRoute] string familyName, [FromBody] FileRequest request)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                service.RevertTree(request.TemplateFilePath);
                SuccessResponse response = new()
                {
                    Message = $"{familyName} tree has been reverted successfully."
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                ExceptionResponse response = APIUtils.SerializeErrorResponse(ex);
                return ex is FileNotFoundException ? BadRequest(response) : StatusCode(500, response);
            }
        }
    }
}
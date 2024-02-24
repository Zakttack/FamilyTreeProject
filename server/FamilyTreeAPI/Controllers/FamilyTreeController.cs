using System.Linq;
using System.Runtime.CompilerServices;
using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
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

        [HttpPost("appendtree")]
        public IActionResult AppendTree([FromBody] AppendTestRequest request)
        {
            try
            {
                FamilyTreeService service = new(request.FamilyName);
                service.AppendTree(request.TemplateFilePath);
                return Ok("The collection has been appended.");
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(APIUtils.SerializeErrorResponse(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }

        [HttpPatch("{familyName}/reportmarried")]
        public IActionResult ReportMarried(string familyName, [FromBody] FamilyElement request)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                Person member = new(request.Member.Name, 
                request.Member.BirthDate is null || request.Member.BirthDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.Member.BirthDate),
                request.Member.DeceasedDate is null || request.Member.DeceasedDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.Member.DeceasedDate));
                Person inLaw = new(request.InLaw.Name,
                request.InLaw.BirthDate is null || request.InLaw.BirthDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.InLaw.BirthDate),
                request.InLaw.DeceasedDate is null || request.InLaw.DeceasedDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.InLaw.DeceasedDate));
                FamilyTreeDate marriageDate = request.MarriageDate is null || request.MarriageDate == "" ? FamilyTreeDate.DefaultDate : new(request.MarriageDate);
                service.ReportMarried(member, inLaw, marriageDate);
                return Ok($"The marriage between {request.Member.Name} and {request.InLaw.Name} has been applied to the tree.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(APIUtils.SerializeErrorResponse(ex));
            }
            catch (FormatException ex)
            {
                return BadRequest(APIUtils.SerializeErrorResponse(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }
    }
}
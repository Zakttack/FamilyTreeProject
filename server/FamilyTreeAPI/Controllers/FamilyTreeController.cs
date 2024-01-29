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
        [HttpGet]
        public IActionResult GetHelloWorld()
        {
            return Ok("Hello World!");
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
                return StatusCode(500, $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
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
                return StatusCode(500, ex);
            }
        }

        [HttpPost("appendtree")]
        public IActionResult AppendTree([FromBody] AppendTestRequest request)
        {
            try
            {
                FamilyTreeService service = new(request.FamilyName);
                service.AppendTree(request.TemplateFilePath);
                return Ok("The collected has been appended.");
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPatch("{familyName}/reportmarried")]
        public IActionResult ReportMarried(string familyName, [FromBody] ReportMarriedRequest request)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                Person member = new(request.MemberName, 
                request.MemberBirthDate is null || request.MemberBirthDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.MemberBirthDate),
                request.MemberDeceasedDate is null || request.MemberDeceasedDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.MemberDeceasedDate));
                Person inLaw = new(request.InLawName,
                request.InLawBirthDate is null || request.InLawBirthDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.InLawBirthDate),
                request.InLawDeceasedDate is null || request.InLawDeceasedDate == "" ? FamilyTreeDate.DefaultDate : new FamilyTreeDate(request.InLawDeceasedDate));
                FamilyTreeDate marriageDate = request.MarriageDate is null || request.MarriageDate == "" ? FamilyTreeDate.DefaultDate : new(request.MarriageDate);
                service.ReportMarried(member, inLaw, marriageDate);
                return Ok($"The marriage between {request.MemberName} and {request.InLawName} has been applied to the tree.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
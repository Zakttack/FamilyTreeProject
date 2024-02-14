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

        [HttpGet("{familyName}/getfamilies/{orderOption}")]
        public IActionResult GetFamilies(string familyName, string orderOption)
        {
            try
            {
                FamilyTreeService service = new(familyName);
                return orderOption switch
                {
                    "parent first then children" => Ok(service.ParentFirstThenChildren),
                    "ascending by name" => Ok(service.AscendingByName),
                    _ => BadRequest("There is nothing to show if you don't select an ordering option."),
                };
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return StatusCode(500, SerializeErrorResponse(ex));
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
                return StatusCode(500, SerializeErrorResponse(ex));
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
                return StatusCode(500, SerializeErrorResponse(ex));
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
                return BadRequest(SerializeErrorResponse(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, SerializeErrorResponse(ex));
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
                return BadRequest(SerializeErrorResponse(ex));
            }
            catch (FormatException ex)
            {
                return BadRequest(SerializeErrorResponse(ex));
            }
            catch (Exception ex)
            {
                return StatusCode(500, SerializeErrorResponse(ex));
            }
        }

        private static IEnumerable<FamilyElement> SerializeFamilyElements(IEnumerable<Family> families)
        {
            ICollection<FamilyElement> familyElements = new List<FamilyElement>();
            foreach (Family family in families)
            {
                FamilyElement familyElement = new()
                {
                    Member = new PersonElement
                    {
                        Name = family.Member.Name,
                        BirthDate = family.Member.BirthDate == FamilyTreeDate.DefaultDate ? null : family.Member.BirthDate.ToString(),
                        DeceasedDate = family.Member.DeceasedDate == FamilyTreeDate.DefaultDate ? null : family.Member.DeceasedDate.ToString()
                    }
                };
                if (family.InLaw is not null)
                {
                    familyElement.InLaw = new PersonElement
                    {
                        Name = family.InLaw.Name,
                        BirthDate = family.InLaw.BirthDate == FamilyTreeDate.DefaultDate ? null : family.InLaw.BirthDate.ToString(),
                        DeceasedDate = family.InLaw.DeceasedDate == FamilyTreeDate.DefaultDate ? null : family.InLaw.DeceasedDate.ToString()
                    };
                }
                familyElement.MarriageDate = family.MarriageDate == FamilyTreeDate.DefaultDate ? null : family.MarriageDate.ToString();
                familyElements.Add(familyElement);
            }
            return familyElements;
        }
        private static ExceptionResponse SerializeErrorResponse(Exception ex)
        {
            return new ExceptionResponse
            {
                Name = ex.GetType().Name,
                Message = ex.Message
            };
        }
    }
}
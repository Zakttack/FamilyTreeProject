using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubFamilyTreeController : ControllerBase
    {
        [HttpPost]
        public IActionResult AnalyzeRepresentation([FromBody] FamilyRepresentationElement represenationElement)
        {
            try
            {
                Family family = new(represenationElement.Representation);
                FamilyElement element = new()
                {
                    Member = new PersonElement
                    {
                        Name = family.Member.Name,
                        BirthDate = family.Member.BirthDate != FamilyTreeDate.DefaultDate ? family.Member.BirthDate.ToString() : null,
                        DeceasedDate = family.Member.DeceasedDate != FamilyTreeDate.DefaultDate ? family.Member.DeceasedDate.ToString() : null
                    },
                    InLaw = family.InLaw is not null ? new PersonElement
                    {
                        Name = family.InLaw.Name,
                        BirthDate = family.InLaw.BirthDate != FamilyTreeDate.DefaultDate ? family.InLaw.BirthDate.ToString() : null,
                        DeceasedDate = family.InLaw.DeceasedDate != FamilyTreeDate.DefaultDate ? family.InLaw.DeceasedDate.ToString() : null
                    } : null,
                    MarriageDate = family.MarriageDate != FamilyTreeDate.DefaultDate ? family.MarriageDate.ToString() : null
                };
                return Ok(element);
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }
    }
}
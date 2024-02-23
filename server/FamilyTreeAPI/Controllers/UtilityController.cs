using FamilyTreeAPI.Models;
using FamilyTreeLibrary;
using FamilyTreeLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : ControllerBase
    {
        [HttpPost("represention-to-element")]
        public IActionResult RepresentationToElement([FromBody] FamilyRepresentationElement represenationElement)
        {
            try
            {
                Family family = new(represenationElement.Representation);
                return Ok(APIUtils.SerializeFamily(family));
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }

        [HttpPost("element-to-representation")]
        public IActionResult ElementToRepresenation([FromBody] FamilyElement element)
        {
            try
            {
                Family family = APIUtils.DeserializeFamilyElement(element);
                FamilyRepresentationElement representation = new()
                {
                    Representation = family.ToString()
                };
                return Ok(representation);
            }
            catch (Exception ex)
            {
                FamilyTreeUtils.WriteError(ex);
                return StatusCode(500, APIUtils.SerializeErrorResponse(ex));
            }
        }
    }
}
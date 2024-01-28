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
    }
}
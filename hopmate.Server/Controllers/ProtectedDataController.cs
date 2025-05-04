using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedDataController : ControllerBase
    {
        [HttpGet("protecteddata")]
        [Authorize]
        public IActionResult GetProtectedData()
        {
            var data = new { message = "This is protected data" };
            return Ok(data);
        }
    }
}
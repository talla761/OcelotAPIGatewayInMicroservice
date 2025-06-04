using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly string[] User = new[]
        {
            "Yvan", "Talla", "LeDoux"
        };

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(User);
        }
    }
}

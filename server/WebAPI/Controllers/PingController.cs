using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/ping/")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}

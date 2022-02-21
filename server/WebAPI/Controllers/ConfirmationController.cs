using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/confirm")]
    [ApiController]
    public class ConfirmationController : ControllerBase
    {
        IIdService idService;
        IConfirmationService confirmationService;

        public ConfirmationController(
            IIdService idService,
            IConfirmationService confirmationService
            )
        {
            this.idService = idService;
            this.confirmationService = confirmationService;
        }

        [HttpGet]
        public IActionResult Confirm(string id)
        {
            id = id.Trim();
            if (!idService.validate(id))
            {
                return BadRequest("Invalid ID format");
            }
            try
            {
                confirmationService.Confirm(id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}

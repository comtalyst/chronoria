using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/confirm")]
    [ApiController]
    public class ConfirmationController : ControllerBase
    {
        IRequestValidationService requestValidationService;
        IConfirmationService confirmationService;

        public ConfirmationController(
            IRequestValidationService requestValidationService,
            IConfirmationService confirmationService
            )
        {
            this.requestValidationService = requestValidationService;
            this.confirmationService = confirmationService;
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(string id)
        {
            id = id.Trim();
            try
            {
                requestValidationService.ValidateId(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            try
            {
                await confirmationService.Confirm(id);
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

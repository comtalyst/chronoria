using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;
using Chronoria_WebAPI.Models;

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
            try
            {
                id = id.Trim();
                requestValidationService.ValidateId(id);
                await confirmationService.Confirm(id);

            }
            catch (RejectException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // TODO: log ex
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }
}

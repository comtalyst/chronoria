using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/cancel")]
    [ApiController]
    public class CancelationController : ControllerBase
    {
        ICancelationService cancelationService;
        IRequestValidationService requestValidationService;
        IIdMatchingService<ActiveContext> idMatchingService;

        public CancelationController(
            ICancelationService cancelationService,
            IRequestValidationService requestValidationService,
            IIdMatchingService<ActiveContext> idMatchingService
            )
        {
            this.cancelationService = cancelationService;
            this.requestValidationService = requestValidationService;
            this.idMatchingService = idMatchingService;
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(string id, string recipientEmail)
        {
            try
            {
                id = id.Trim();
                recipientEmail = recipientEmail.Trim();
                requestValidationService.ValidateId(id);
                requestValidationService.ValidateEmail(recipientEmail);

                if(!await idMatchingService.MatchRecipientEmail(id, recipientEmail))
                {
                    throw new RejectException(RejectException.VerificationFailed);
                }

                await cancelationService.Cancel(id);
            }
            catch (RejectException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}

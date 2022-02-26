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
        public async Task<IActionResult> Cancel(string id, string receipientEmail)
        {
            try
            {
                id = id.Trim();
                receipientEmail = receipientEmail.Trim();
                requestValidationService.ValidateId(id);
                requestValidationService.ValidateEmail(receipientEmail);

                if(!await idMatchingService.MatchReceipientEmail(id, receipientEmail))
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
                // TODO: log ex
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/download")]
    [ApiController]
    public class DownloadLinkController : ControllerBase
    {
        //IDownloadLinkService downloadLinkService;
        IRequestValidationService requestValidationService;
        IIdMatchingService<ActiveContext> idMatchingService;

        public DownloadLinkController(
            //IDownloadLinkService downloadLinkService,
            IRequestValidationService requestValidationService,
            IIdMatchingService<ActiveContext> idMatchingService
            )
        {
            //this.downloadLinkService = downloadLinkService;
            this.requestValidationService = requestValidationService;
            this.idMatchingService = idMatchingService;
        }

        [HttpGet]
        public async Task<IActionResult> Download(string id, string recipientEmail)
        {
            try
            {
                id = id.Trim();
                recipientEmail = recipientEmail.Trim();
                requestValidationService.ValidateId(id);
                requestValidationService.ValidateEmail(recipientEmail);

                if (!await idMatchingService.MatchRecipientEmail(id, recipientEmail))
                {
                    throw new RejectException(RejectException.VerificationFailed);
                }
                throw new NotImplementedException();
                //return Ok(await downloadLinkService.GetLink(id));
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
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/downloadlink")]
    [ApiController]
    public class DownloadLinkController : ControllerBase
    {
        IDownloadLinkService downloadLinkService;
        IRequestValidationService requestValidationService;
        IIdMatchingService<ArchivedContext> idMatchingService;

        public DownloadLinkController(
            IDownloadLinkService downloadLinkService,
            IRequestValidationService requestValidationService,
            IIdMatchingService<ArchivedContext> idMatchingService
            )
        {
            this.downloadLinkService = downloadLinkService;
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
                return Ok(await downloadLinkService.GetLink(id));
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
        }
    }
}

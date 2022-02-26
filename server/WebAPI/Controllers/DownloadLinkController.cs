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
        public async Task<IActionResult> Download(string id, string receipientEmail)
        {
            try
            {
                id = id.Trim();
                receipientEmail = receipientEmail.Trim();
                requestValidationService.ValidateId(id);
                requestValidationService.ValidateEmail(receipientEmail);

                if (!await idMatchingService.MatchReceipientEmail(id, receipientEmail))
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

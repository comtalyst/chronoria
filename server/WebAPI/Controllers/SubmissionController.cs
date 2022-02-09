using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/submit/")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        ISubmissionService submissionService;
        IBlocklistService blocklistService;

        [Route("file/")]
        [HttpPost]
        public async Task<IActionResult> PostFile(
            [FromBody] 
                string senderEmail,
                string senderName,
                string recipientEmail,
                string recipientName,
                long sendTime,
                string textLocation,
                string text,
            [FromForm] 
                Models.UploadedFile file
            )
        {
            // TODO: validate all parameters (for security proposes)--maybe create a validator service and utilize it here

            try
            {
                // Check blocklist
                if (await blocklistService.BlockExists(senderEmail))
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                await submissionService.SubmitFile(
                    senderEmail,
                    senderName,
                    recipientEmail,
                    recipientName,
                    sendTime,
                    textLocation,
                    text,
                    file
                );
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }

        [Route("text/")]
        [HttpPost]
        public async Task<IActionResult> PostText(
            [FromBody]
                string senderEmail,
                string senderName,
                string recipientEmail,
                string recipientName,
                long sendTime,
                string text
            )
        {
            try
            {
                // Check blocklist
                if (await blocklistService.BlockExists(senderEmail))
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                await submissionService.SubmitText(
                    senderEmail,
                    senderName,
                    recipientEmail,
                    recipientName,
                    sendTime,
                    text
                );
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}

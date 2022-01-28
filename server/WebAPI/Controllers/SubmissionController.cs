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

        [Route("file/")]
        [HttpPost]
        public async Task<IActionResult> PostFile(
            [FromBody] 
                string senderEmail,
                string senderName,
                string recipientEmail,
                string recipientName,
                long sendTime,
                long createTime,
                int textLocation,
                string text,
            [FromForm] 
                Models.File file
            )
        {
            try
            {
                submissionService.SubmitFile(
                    senderEmail,
                    senderName,
                    recipientEmail,
                    recipientName,
                    sendTime,
                    createTime,
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
                long createTime,
                string text
            )
        {
            try
            {
                submissionService.SubmitText(
                    senderEmail,
                    senderName,
                    recipientEmail,
                    recipientName,
                    sendTime,
                    createTime,
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

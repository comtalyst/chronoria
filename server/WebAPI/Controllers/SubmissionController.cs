using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Services;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/submit/")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        ISubmissionService submissionService;
        IBlocklistService blocklistService;
        IRequestValidationService requestValidationService;

        public SubmissionController(
            ISubmissionService submissionService,
            IBlocklistService blocklistService,
            IRequestValidationService requestValidationService
            )
        {
            this.submissionService = submissionService;
            this.blocklistService = blocklistService;
            this.requestValidationService = requestValidationService;
        }

        public class PostFileModel
        {
            public string senderEmail { get; set; }
            public string senderName { get; set; }
            public string recipientEmail { get; set; }
            public string recipientName { get; set; }
            public long sendTime { get; set; }
            public string textLocation { get; set; }
            public string text { get; set; }
        }
        [Route("file/")]
        [HttpPost]
        public async Task<IActionResult> PostFile(
            [FromBody] PostFileModel postFileModel,
            [FromForm] UploadedFile file
            )
        {
            var senderEmail = postFileModel.senderEmail.Trim();
            var senderName = postFileModel.senderName.Trim();
            var recipientEmail = postFileModel.recipientEmail.Trim();
            var recipientName = postFileModel.recipientName.Trim();
            var sendTime = postFileModel.sendTime;
            var textLocation = postFileModel.textLocation.Trim();
            var text = postFileModel.text.Trim();
            // validate all parameters (for security proposes)
            try
            {
                requestValidationService.ValidateEmail(senderEmail);
                requestValidationService.ValidateEmail(recipientEmail);
                requestValidationService.ValidateName(senderName);
                requestValidationService.ValidateName(recipientName);
                //requestValidationService.ValidateFutureTime(sendTime);
                requestValidationService.ValidateTextLoc(textLocation);
                requestValidationService.ValidateText(text);
                requestValidationService.ValidateFile(file);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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

        public class PostTextModel
        {
            public string senderEmail { get; set; }
            public string senderName { get; set; }
            public string recipientEmail { get; set; }
            public string recipientName { get; set; }
            public long sendTime { get; set; }
            public string text { get; set; }
        }
        [Route("text/")]
        [HttpPost]
        public async Task<IActionResult> PostText(
            [FromBody] PostTextModel postTextModel
            )
        {
            var senderEmail = postTextModel.senderEmail.Trim();
            var recipientEmail = postTextModel.recipientEmail.Trim();
            var senderName = postTextModel.senderName.Trim();
            var recipientName = postTextModel.recipientName.Trim();
            var sendTime = postTextModel.sendTime;
            var text = postTextModel.text.Trim();
            // validate all parameters (for security proposes)
            try
            {
                requestValidationService.ValidateEmail(senderEmail);
                requestValidationService.ValidateEmail(recipientEmail);
                requestValidationService.ValidateName(senderName);
                requestValidationService.ValidateName(recipientName);
                //requestValidationService.ValidateFutureTime(sendTime);
                requestValidationService.ValidateText(text);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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

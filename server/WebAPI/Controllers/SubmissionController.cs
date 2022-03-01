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
            try
            {
                // Clean parameters
                var senderEmail = postFileModel.senderEmail.Trim();
                var senderName = postFileModel.senderName.Trim();
                var recipientEmail = postFileModel.recipientEmail.Trim();
                var recipientName = postFileModel.recipientName.Trim();
                var sendTime = postFileModel.sendTime;
                var textLocation = postFileModel.textLocation.Trim();
                var text = postFileModel.text.Trim();

                // Validate all parameters (for security proposes)
                requestValidationService.ValidateEmail(senderEmail);
                requestValidationService.ValidateEmail(recipientEmail);
                requestValidationService.ValidateName(senderName);
                requestValidationService.ValidateName(recipientName);
                //requestValidationService.ValidateFutureTime(sendTime);
                requestValidationService.ValidateTextLoc(textLocation);
                requestValidationService.ValidateText(text);
                requestValidationService.ValidateFile(file);

                // Check blocklist
                if (await blocklistService.BlockExists(senderEmail))
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                // Utilize service
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
            try
            {
                // Clean parameters
                var senderEmail = postTextModel.senderEmail.Trim();
                var senderName = postTextModel.senderName.Trim();
                var recipientEmail = postTextModel.recipientEmail.Trim();
                var recipientName = postTextModel.recipientName.Trim();
                var sendTime = postTextModel.sendTime;
                var text = postTextModel.text.Trim();

                // Validate all parameters (for security proposes)
                requestValidationService.ValidateEmail(senderEmail);
                requestValidationService.ValidateEmail(recipientEmail);
                requestValidationService.ValidateName(senderName);
                requestValidationService.ValidateName(recipientName);
                //requestValidationService.ValidateFutureTime(sendTime);
                requestValidationService.ValidateText(text);

                // Check blocklist
                if (await blocklistService.BlockExists(senderEmail))
                {
                    throw new RejectException(RejectException.BlockedSenderEmail);
                }

                // Utilize service
                await submissionService.SubmitText(
                    senderEmail,
                    senderName,
                    recipientEmail,
                    recipientName,
                    sendTime,
                    text
                );
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

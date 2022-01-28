using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Controllers
{
    [Route("api/submit/")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
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
            // TODO: Check blacklist

            // TODO: Reroute the file to blob storage
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    file.FormFile.CopyTo(stream);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // TODO: Generate UUID

            // TODO: Put into DB

            // TODO: Confirmation email

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
            // TODO: Check blacklist

            // TODO: Generate UUID

            // TODO: Put into DB

            // TODO: Confirmation email

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}

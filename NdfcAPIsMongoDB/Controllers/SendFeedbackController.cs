using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hangfire;
using NdfcAPIsMongoDB.Common.EmailService;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class SendFeedbackController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IEmailService _emailService;
        public SendFeedbackController(IBackgroundJobClient backgroundJobClient, IEmailService emailService)
        {
            _backgroundJobClient = backgroundJobClient;
            _emailService = emailService;
        }

        /// <summary>
        /// send feedback to user contact by email
        /// </summary>
        [HttpPost]
        [Authorize]
        public IActionResult Send(string mail, string bodyString)
        {
            _backgroundJobClient.Enqueue(() => _emailService.SendEmail(mail, bodyString));
            return Ok();
        }

        /// <summary>
        /// send message to all subcribers
        /// </summary>
        [HttpPost("sendToAllSubcriber")]
        public IActionResult SendEmailsToAll(string body)
        {
            try
            {
                _emailService.SendEmailsToAll(body);
                return Ok("Emails sent to all subscribers.");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to send emails: " + ex.Message);
            }
        }
        /// <summary>
        /// send email with file
        /// </summary>
        [HttpPost("sendEmailwithfile")]
        public IActionResult SendEmailWithFile(string to, string body, string subject, IFormFile attachmentFile)
        {
            try
            {
                _emailService.SendEmailWithAttachment(to, body, subject, attachmentFile);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}



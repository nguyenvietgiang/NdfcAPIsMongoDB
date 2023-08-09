using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
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
    }
}



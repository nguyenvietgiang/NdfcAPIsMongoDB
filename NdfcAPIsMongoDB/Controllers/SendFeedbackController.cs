using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Authorization;

namespace NdfcAPIsMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendFeedbackController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public IActionResult Send(string mail, string bodyString)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("nguyenvietgiang1110@gmail.com"));
            message.To.Add(MailboxAddress.Parse(mail));
            message.Subject = "Câu lạc bộ bóng đá tỉnh Nam Định";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <html>
                    <head>
                        <title>Bức Thư</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                font-size: 14px;
                                line-height: 1.5;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #ccc;
                                border-radius: 5px;
                            }}
                            .header {{
                                text-align: center;
                                margin-bottom: 20px;
                            }}
                            .logo {{
                                max-width: 200px;
                                max-height: 200px;
                                display: block;
                                margin: 0 auto;
                            }}
                            .content {{
                                margin-bottom: 20px;
                            }}
                            .footer {{
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <img class=""logo"" src=""https://upload.wikimedia.org/wikipedia/vi/thumb/8/89/Nam_%C4%90%E1%BB%8Bnh_FC_logo.svg/1200px-Nam_%C4%90%E1%BB%8Bnh_FC_logo.svg.png"" alt=""Logo"">
                                <h2>Câu lạc bộ bóng đá tỉnh Nam Định</h2>
                            </div>
                            <div class=""content"">
                                <p>Xin chào!</p>
                                <p>Dưới đây là nội dung email gửi từ Câu lạc bộ bóng đá tỉnh Nam Định.</p>
                                <p>{bodyString}</p>
                                <p>Cảm ơn bạn đã gửi phản hồi!</p>
                            </div>
                            <div class=""footer"">
                                <p>Trân trọng,</p>
                                <p>Nguyễn Việt Giang</p>
                            </div>
                        </div>
                    </body>
                </html>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("nguyenvietgiang1110@gmail.com", "kholeeizbmxzykbs");
                client.Send(message);
                client.Disconnect(true);
            }

            return Ok();
        }
    }
}


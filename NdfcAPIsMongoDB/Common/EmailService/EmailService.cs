using MimeKit;
using MailKit.Net.Smtp;
using MongoDB.Driver;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Common.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IMongoCollection<Subscriber> _subCollection;

        public EmailService(IMongoDatabase database)
        {
            _subCollection = database.GetCollection<Subscriber>("Subscriber");
        }

        public void SendEmailsToAll(string body)
        {
            var emailCollection = _subCollection;

            // Lấy tất cả các email từ MongoDB
            var filter = Builders<Subscriber>.Filter.Empty;
            var subscribers = emailCollection.Find(filter).ToList();

            foreach (var subscriber in subscribers)
            {
                // Gửi email cho từng địa chỉ email trong danh sách
                SendEmail(subscriber.Email, body);
            }
        }
        // dịch vụ gửi email kèm theo file đính kèm
        public void SendEmailWithAttachment(string to, string body, string subject, IFormFile attachmentFile)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("nguyenvietgiang1110@gmail.com"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = "Câu lạc bộ bóng đá tỉnh Nam Định";
            var builder = new BodyBuilder();

            builder.TextBody = body;

            if (attachmentFile != null && attachmentFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Đọc dữ liệu từ IFormFile vào MemoryStream
                    attachmentFile.CopyTo(memoryStream);
                    // Thêm tệp tin vào email
                    var attachment = builder.Attachments.Add(attachmentFile.FileName, memoryStream.ToArray());
                    // Set the attachment's Content-Disposition parameters
                    attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                    attachment.ContentDisposition.FileName = attachmentFile.FileName;
                }
            }
            message.Body = builder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("nguyenvietgiang1110@gmail.com", "********");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void SendEmail(string mail, string bodyString)
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
                client.Authenticate("nguyenvietgiang1110@gmail.com", "********");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}

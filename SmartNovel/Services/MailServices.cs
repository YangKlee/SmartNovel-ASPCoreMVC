
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace SmartNovel.Services
{
    public class MailServices
    {
        private readonly IConfiguration _config;


        public MailServices(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string emailTaget, string subject, string body)
        {
            string senderName = _config["EmailSettings:SenderName"];
            string senderEmail = _config["EmailSettings:SenderEmail"];
            string password = _config["EmailSettings:Password"];
            string smtpServer = _config["EmailSettings:SmtpServer"];
            int smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(MailboxAddress.Parse(emailTaget));
            email.Subject = subject;
            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, password);
                await smtp.SendAsync(email);
                return true;

            }
            catch
            {
                return false;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }


        }
    }
}

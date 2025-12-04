using System.Net;
using System.Net.Mail;
using MassAidVOne.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace MassAidVOne.Application.Services
{
    public class EmailSetting
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _emailSetting;

        public EmailService(IOptions<EmailSetting> emailSetting)
        {
            _emailSetting = emailSetting.Value;
        }

        public async Task SendEmailAsync(string ToEmail, string Subject, string Body)
        {
            using (var client = new SmtpClient(_emailSetting.SmtpServer, _emailSetting.SmtpPort))
            {
                client.Credentials = new NetworkCredential(_emailSetting.SenderEmail, _emailSetting.Password);
                client.EnableSsl = true;
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSetting.SenderEmail, _emailSetting.SenderName),
                    Body = Body,
                    Subject = Subject,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(ToEmail);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}

using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Practice.Core.Services
{
    public class EmailService : IIdentityMessageService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            var from = _smtpClient.Credentials.GetCredential(_smtpClient.Host, _smtpClient.Port, "").UserName;
            var mail = new MailMessage(from, message.Destination, message.Subject, message.Body);
            mail.IsBodyHtml = true;
            await _smtpClient.SendMailAsync(mail);
        }
    }
}

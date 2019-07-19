using System.Net.Mail;
using System.Threading.Tasks;

namespace Practice.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var mail = new MailMessage("onex.practice2019@gmail.com", message.Destination, message.Subject, message.Body);
            
            await _smtpClient.SendMailAsync(mail);
        }
    }
}

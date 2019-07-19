using System.Threading.Tasks;

namespace Practice.Core.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }
}

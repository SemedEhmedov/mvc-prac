using Uniqlo.Helpers.Email;

namespace Uniqlo.Abstrcations.EmailServices
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}

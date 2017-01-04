using System.Net.Mail;

namespace TAL.QuoteAndApply.Infrastructure.Mail
{
    public interface ISmtpClientFactory
    {
        SmtpClient GetClient(int sendTimeout);
    }

    public class SmtpClientFactory : ISmtpClientFactory
    {
        public SmtpClient GetClient(int sendTimeout)
        {
            return new SmtpClient { Timeout = sendTimeout };
        }
    }
}
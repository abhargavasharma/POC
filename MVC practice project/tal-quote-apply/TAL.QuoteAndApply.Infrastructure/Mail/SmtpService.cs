using System;
using System.Net.Mail;
using log4net;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.Infrastructure.Mail
{
    public interface ISmtpService
    {
        void SendMessage(int sendTimeout, string body, string to, string from, string subject, string bcc, Attachment attachment = null);
    }

    public class SmtpService : ISmtpService
    {
        private readonly ILoggingService _logger;
        private readonly ISmtpClientFactory _smtpClientFactory;
        private readonly IMailMessageBuilder _mailMessageBuilder;

        public SmtpService(ILoggingService logger, ISmtpClientFactory smtpClientFactory, IMailMessageBuilder mailMessageBuilder)
        {
            _logger = logger;
            _smtpClientFactory = smtpClientFactory;
            _mailMessageBuilder = mailMessageBuilder;
        }

        public void SendMessage(int sendTimeout, string body, string to, string from, string subject, string bcc, Attachment attachment = null)
        {
            var client = _smtpClientFactory.GetClient(sendTimeout);
            var msg = _mailMessageBuilder.BuildMessage(to, from, subject, body, attachment);
            if (!string.IsNullOrEmpty(bcc))
            {
                msg.Bcc.Add(bcc);
            }

            client.SendCompleted += (s, e) =>
            {
                client.Dispose();
                msg.Dispose();
            };

            try
            {
                client.Send(msg);
            }
            catch (SmtpFailedRecipientException)
            {
                _logger.Info("Failed to send to recipient");
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to send mail", ex);
                throw;
            }
        }


    }
}

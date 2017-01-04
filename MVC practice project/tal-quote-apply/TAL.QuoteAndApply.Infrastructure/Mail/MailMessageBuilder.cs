using System.Net.Mail;

namespace TAL.QuoteAndApply.Infrastructure.Mail
{
    public interface IMailMessageBuilder
    {
        MailMessage BuildMessage(string to, string from, string subject, string body, Attachment attachment = null);
    }

    public class MailMessageBuilder : IMailMessageBuilder
    {
        public MailMessage BuildMessage(string to, string from, string subject, string body, Attachment attachment = null)
        {
            var msg = new MailMessage(from, to)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            };
            
            if (attachment != null)
            {
                msg.Attachments.Add(attachment);
            }

            return msg;
        }   
    }
}
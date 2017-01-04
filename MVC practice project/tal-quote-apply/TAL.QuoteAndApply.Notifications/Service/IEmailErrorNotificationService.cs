using System.Collections.Generic;

namespace TAL.QuoteAndApply.Notifications.Service
{
    public interface IEmailErrorNotificationService
    {
        void SendRaisePolicyErrorEmail(string quoteReferenceNumber, string errorMessage, string emailToAddress);
        void SendRaisePolicyValidationErrorEmail(string quoteReferenceNumber, IEnumerable<string> validations, string emailToAddress);
    }
}
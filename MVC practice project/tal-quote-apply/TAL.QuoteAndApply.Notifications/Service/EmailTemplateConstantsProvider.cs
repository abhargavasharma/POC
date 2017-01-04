namespace TAL.QuoteAndApply.Notifications.Service
{
    public class EmailTemplateConstantsProvider : IEmailTemplateConstantsProvider
    {
        public string GetEmailQuoteTemplateByBrand(string brand)
        {
            return $"TAL.QuoteAndApply.Notifications.Templates.{brand.ToUpper()}EmailQuote.cshtml";
        }
        public string GetEmailSaveQuoteTemplateByBrand(string brand)
        {
            return $"TAL.QuoteAndApply.Notifications.Templates.{brand.ToUpper()}EmailSaveQuote.cshtml";
        }
        public string GetEmailApplicationConfirmationTemplateByBrand(string brand)
        {
            return $"TAL.QuoteAndApply.Notifications.Templates.{brand.ToUpper()}EmailApplicationConfirmation.cshtml";
        }
        public string GetProtectionQuoteSubjectTextByBrand(string brand)
        {
            return $"{brand.ToUpper()} Lifetime Protection Quote";
        }
        public string GetApplicationConfirmationSubjectByBrand(string brand)
        {
            return $"Welcome to {brand.ToUpper()} Lifetime Protection";
        }
    }
}
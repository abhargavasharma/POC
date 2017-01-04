namespace TAL.QuoteAndApply.Notifications.Service
{
    public interface IEmailTemplateConstantsProvider
    {
        string GetEmailApplicationConfirmationTemplateByBrand(string brand);
        string GetEmailQuoteTemplateByBrand(string brand);
        string GetEmailSaveQuoteTemplateByBrand(string brand);
        string GetProtectionQuoteSubjectTextByBrand(string brand);
        string GetApplicationConfirmationSubjectByBrand(string brand);
    }
}
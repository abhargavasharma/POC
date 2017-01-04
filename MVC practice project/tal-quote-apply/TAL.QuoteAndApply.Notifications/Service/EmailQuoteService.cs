using System.Reflection;
using RazorEngine;
using RazorEngine.Templating;
using TAL.QuoteAndApply.Infrastructure.Mail;
using TAL.QuoteAndApply.Infrastructure.Resource;
using TAL.QuoteAndApply.Notifications.Configuration;
using TAL.QuoteAndApply.Notifications.Models;

namespace TAL.QuoteAndApply.Notifications.Service
{
    public interface IEmailQuoteService
    {
        bool SendQuote(string quoteReferenceNumber, string emailAddress, string firstName, string brandKey);
        bool SendSalesPortalQuoteSavedEmail(EmailQuoteSavedViewModel emailQuoteSavedViewModel, string quoteReferenceNumber);
        bool SendSalesPortalApplicationConfirmationEmail(EmailQuoteSavedViewModel emailQuoteSavedViewModel, string quoteReferenceNumber);
    }

    public class EmailQuoteService : IEmailQuoteService
    {
        private readonly ISmtpService _mailService;
        private readonly IResourceFileReader _resourceFileReader;
        private readonly IEmailQuoteViewModelProvider _emailQuoteViewModelProvider;
        private readonly IEmailConfigurationProvider _emailConfigurationProvider;
        private readonly IEmailTemplateConstantsProvider _emailTemplateConstantsProvider;

        public EmailQuoteService(ISmtpService mailService, 
            IResourceFileReader resourceFileReader,
            IEmailQuoteViewModelProvider emailQuoteViewModelProvider, 
            IEmailConfigurationProvider emailConfigurationProvider, 
            IEmailTemplateConstantsProvider emailTemplateConstantsProvider)
        {
            _mailService = mailService;
            _resourceFileReader = resourceFileReader;
            _emailQuoteViewModelProvider = emailQuoteViewModelProvider;
            _emailConfigurationProvider = emailConfigurationProvider;
            _emailTemplateConstantsProvider = emailTemplateConstantsProvider;
        }

        public bool SendQuote(string quoteReferenceNumber, string emailAddress, string firstName, string brandKey)
        {
            var viewModel = _emailQuoteViewModelProvider.From(quoteReferenceNumber, emailAddress, firstName, _emailConfigurationProvider.NoReplyEmailAddress, brandKey);

            if (viewModel.AllowEmail)
            {
                var razorContent = _resourceFileReader.GetContentsOfResource(Assembly.GetAssembly(GetType()),
                    _emailTemplateConstantsProvider.GetEmailQuoteTemplateByBrand(brandKey));
                var emailContent = Engine.Razor.RunCompile(razorContent, _emailTemplateConstantsProvider.GetEmailQuoteTemplateByBrand(brandKey), null,
                    viewModel);

                _mailService.SendMessage(20000, emailContent, emailAddress, viewModel.UserEmailAddress, _emailTemplateConstantsProvider.GetProtectionQuoteSubjectTextByBrand(brandKey), "");
            }
            return viewModel.AllowEmail;
        }

        public bool SendSalesPortalQuoteSavedEmail(EmailQuoteSavedViewModel emailQuoteSavedViewModel, string quoteReferenceNumber)
        {
            emailQuoteSavedViewModel.AgentEmailAddress = emailQuoteSavedViewModel.AgentEmailAddress ?? _emailConfigurationProvider.DefaultSenderEmailAddress;
            emailQuoteSavedViewModel.BaseEmailModel = _emailQuoteViewModelProvider.From(quoteReferenceNumber, emailQuoteSavedViewModel.BaseEmailModel.EmailAddress, 
                emailQuoteSavedViewModel.BaseEmailModel.FirstName, emailQuoteSavedViewModel.AgentEmailAddress, emailQuoteSavedViewModel.Brandkey);

            if (emailQuoteSavedViewModel.BaseEmailModel.AllowEmail)
            {
                var razorContent = _resourceFileReader.GetContentsOfResource(Assembly.GetAssembly(GetType()),
                    _emailTemplateConstantsProvider.GetEmailSaveQuoteTemplateByBrand(emailQuoteSavedViewModel.Brandkey));
                var emailContent = Engine.Razor.RunCompile(razorContent, _emailTemplateConstantsProvider.GetEmailSaveQuoteTemplateByBrand(emailQuoteSavedViewModel.Brandkey),
                    null, emailQuoteSavedViewModel);

                _mailService.SendMessage(20000, emailContent, emailQuoteSavedViewModel.BaseEmailModel.EmailAddress,
                    emailQuoteSavedViewModel.BaseEmailModel.UserEmailAddress, _emailTemplateConstantsProvider.GetProtectionQuoteSubjectTextByBrand(emailQuoteSavedViewModel.Brandkey), 
                    _emailConfigurationProvider.DefaultBccEmailAddress);
            }
            return emailQuoteSavedViewModel.BaseEmailModel.AllowEmail;
        }

        public bool SendSalesPortalApplicationConfirmationEmail(EmailQuoteSavedViewModel emailQuoteSavedViewModel,
            string quoteReferenceNumber)
        {
            emailQuoteSavedViewModel.AgentEmailAddress = emailQuoteSavedViewModel.AgentEmailAddress ?? _emailConfigurationProvider.DefaultSenderEmailAddress;
            emailQuoteSavedViewModel.BaseEmailModel = _emailQuoteViewModelProvider.From(quoteReferenceNumber, emailQuoteSavedViewModel.BaseEmailModel.EmailAddress,
                emailQuoteSavedViewModel.BaseEmailModel.FirstName, emailQuoteSavedViewModel.AgentEmailAddress, emailQuoteSavedViewModel.Brandkey);

            if (emailQuoteSavedViewModel.BaseEmailModel.AllowEmail)
            {

                var razorContent = _resourceFileReader.GetContentsOfResource(Assembly.GetAssembly(GetType()),
                    _emailTemplateConstantsProvider.GetEmailSaveQuoteTemplateByBrand(emailQuoteSavedViewModel.Brandkey));
                var emailContent = Engine.Razor.RunCompile(razorContent,
                    _emailTemplateConstantsProvider.GetEmailSaveQuoteTemplateByBrand(emailQuoteSavedViewModel.Brandkey), null, emailQuoteSavedViewModel);

                _mailService.SendMessage(20000, emailContent, emailQuoteSavedViewModel.BaseEmailModel.EmailAddress,
                    emailQuoteSavedViewModel.BaseEmailModel.UserEmailAddress,
                    _emailTemplateConstantsProvider.GetApplicationConfirmationSubjectByBrand(emailQuoteSavedViewModel.Brandkey), _emailConfigurationProvider.DefaultBccEmailAddress);
            }
            return emailQuoteSavedViewModel.BaseEmailModel.AllowEmail;
        }
    }
}

using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using RazorEngine;
using RazorEngine.Templating;
using TAL.QuoteAndApply.Infrastructure.Mail;
using TAL.QuoteAndApply.Infrastructure.Resource;
using TAL.QuoteAndApply.Notifications.Models;

namespace TAL.QuoteAndApply.Notifications.Service
{
    public class EmailErrorNotificationService : IEmailErrorNotificationService
    {
        private readonly ISmtpService _mailService;
        private readonly IResourceFileReader _resourceFileReader;

        public EmailErrorNotificationService(ISmtpService mailService, IResourceFileReader resourceFileReader)
        {
            _mailService = mailService;
            _resourceFileReader = resourceFileReader;
        }

        public void SendRaisePolicyErrorEmail(string quoteReferenceNumber, string errorMessage, string emailToAddress)
        {
            var viewModel = new RaisePolicyErrorViewModel
            {
                QuoteReferenceNumber = quoteReferenceNumber,
                ErrorMessage = errorMessage,
                Environment = ConfigurationManager.AppSettings["EnvironmentName"]
            };

            var razorContent = _resourceFileReader.GetContentsOfResource(Assembly.GetAssembly(GetType()), EmailErrorNotificationTemplateConstansts.RaisePolicyErrorEmailTemplate);
            var emailContent = Engine.Razor.RunCompile(razorContent, EmailErrorNotificationTemplateConstansts.RaisePolicyErrorEmailTemplate, null, viewModel);

            _mailService.SendMessage(20000, emailContent, emailToAddress, "no-reply@tal.com.au", "[TAL Consumer] - Raise Policy Error", null);
        }

        public void SendRaisePolicyValidationErrorEmail(string quoteReferenceNumber, IEnumerable<string> validations, string emailToAddress)
        {
            var viewModel = new RaisePolicyValidationErrorViewModel
            {
                QuoteReferenceNumber = quoteReferenceNumber,
                Validations = validations,
                Environment = ConfigurationManager.AppSettings["EnvironmentName"]
            };

            var razorContent = _resourceFileReader.GetContentsOfResource(Assembly.GetAssembly(GetType()), EmailErrorNotificationTemplateConstansts.RaisePolicyValidationErrorEmailTemplate);
            var emailContent = Engine.Razor.RunCompile(razorContent, EmailErrorNotificationTemplateConstansts.RaisePolicyValidationErrorEmailTemplate, null, viewModel);

            _mailService.SendMessage(20000, emailContent, emailToAddress, "no-reply@tal.com.au",  "[TAL Consumer] - Raise Policy Validation Error", null);
        }
    }
}

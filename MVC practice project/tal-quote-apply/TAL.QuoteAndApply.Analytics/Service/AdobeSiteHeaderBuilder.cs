using System.Reflection;
using RazorEngine;
using RazorEngine.Templating;
using TAL.QuoteAndApply.Analytics.Models;
using TAL.QuoteAndApply.Analytics.Templates;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Resource;

namespace TAL.QuoteAndApply.Analytics.Service
{
    public interface IAdobeSiteHeaderBuilder
    {
        string GetCode(AdobeHeaderViewModel viewModel);
    }

    public class AdobeSiteHeaderBuilder : IAdobeSiteHeaderBuilder
    {
        private readonly IResourceFileReader _resourceFileReader;

        public AdobeSiteHeaderBuilder(IResourceFileReader resourceFileReader)
        {
            _resourceFileReader = resourceFileReader;
        }

        public string GetCode(AdobeHeaderViewModel viewModel)
        {
            var razorContent = _resourceFileReader.GetContentsOfResource(Assembly.GetAssembly(GetType()), TemplateConstants.AdobeSiteHeaderTemplate);
            return Engine.Razor.RunCompile(razorContent, TemplateConstants.AdobeSiteHeaderTemplate, null, viewModel);
        }
    }
}

using System.Reflection;
using RazorEngine;
using RazorEngine.Templating;
using TAL.QuoteAndApply.Analytics.Models;
using TAL.QuoteAndApply.Analytics.Templates;
using TAL.QuoteAndApply.Infrastructure.Resource;

namespace TAL.QuoteAndApply.Analytics.Service
{
    public interface IAdobeSiteFooterBuilder
    {
        string GetCode();
    }

    public class AdobeSiteFooterBuilder : IAdobeSiteFooterBuilder
    {
        private readonly IResourceFileReader _resourceFileReader;

        public AdobeSiteFooterBuilder(IResourceFileReader resourceFileReader)
        {
            _resourceFileReader = resourceFileReader;
        }

        public string GetCode()
        {
            var razorContent = _resourceFileReader.GetContentsOfResource(Assembly.GetAssembly(GetType()), TemplateConstants.AdobeSiteFooterTemplate);
            return Engine.Razor.RunCompile(razorContent, TemplateConstants.AdobeSiteFooterTemplate, null, new object());
        }
    }
}
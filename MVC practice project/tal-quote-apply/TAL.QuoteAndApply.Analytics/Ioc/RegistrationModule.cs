using TAL.QuoteAndApply.Analytics.Configuration;
using TAL.QuoteAndApply.Analytics.Service;
using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Infrastructure.Resource;

namespace TAL.QuoteAndApply.Analytics.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<AdobeSiteHeaderBuilder>();
            mapper.Add<AdobeSiteHeaderBuilder>();
            mapper.Add<AdobeSiteFooterBuilder>();
            mapper.Add<AdobeHeaderViewModelConverter>();
            mapper.Add<AnalyticsConfiguration>();
            mapper.Add<AnalyticsSiteHeaderBuilder>();
        }
    }
}

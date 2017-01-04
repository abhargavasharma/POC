using TAL.QuoteAndApply.Analytics.Models;

namespace TAL.QuoteAndApply.Analytics.Service
{
    public interface IAnalyticsSiteHeaderBuilder
    {
        string BuildHeader(AnalyticsPageModel pageModel);
        string BuildFooter();
    }

    public class AnalyticsSiteHeaderBuilder : IAnalyticsSiteHeaderBuilder
    {
        private readonly IAdobeSiteHeaderBuilder _adobeSiteHeaderBuilder;
        private readonly IAdobeSiteFooterBuilder _adobeSiteFooterBuilder;
        private readonly IAdobeHeaderViewModelConverter _adobeHeaderViewModelConverter;

        public AnalyticsSiteHeaderBuilder(IAdobeSiteHeaderBuilder adobeSiteHeaderBuilder, IAdobeHeaderViewModelConverter adobeHeaderViewModelConverter, IAdobeSiteFooterBuilder adobeSiteFooterBuilder)
        {
            _adobeSiteHeaderBuilder = adobeSiteHeaderBuilder;
            _adobeHeaderViewModelConverter = adobeHeaderViewModelConverter;
            _adobeSiteFooterBuilder = adobeSiteFooterBuilder;
        }

        public string BuildHeader(AnalyticsPageModel pageModel)
        {
            var viewModel = _adobeHeaderViewModelConverter.GetFor(pageModel);
            return _adobeSiteHeaderBuilder.GetCode(viewModel);
        }

        public string BuildFooter()
        {
            return _adobeSiteFooterBuilder.GetCode();
        }
    }
}

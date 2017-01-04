using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Monads;
using System.Web;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Web.Shared.Configuration;

namespace TAL.QuoteAndApply.Customer.Web.Configuration
{
    public interface ICaptchaConfigurationProvider
    {
        bool IsEnabled { get; }

        string GoogleCaptchaPrivateKey { get; }

        string GoogleCaptchaBaseUrl { get;  }
    }

    public class CaptchaConfigurationProvider : ICaptchaConfigurationProvider
    {
        private const string IgnoreRecaptchaParamName = "ignore.recaptcha";

        private readonly IWebConfiguration _webConfiguration;
        private readonly IHttpContextProvider _httpContextProvider;

        public CaptchaConfigurationProvider(IWebConfiguration webConfiguration, IHttpContextProvider httpContextProvider)
        {
            _webConfiguration = webConfiguration;
            _httpContextProvider = httpContextProvider;
        }

        public bool IsEnabled
        {
            get
            {
                var captchaEnabled = bool.Parse(ConfigurationManager.AppSettings["Google.IsCaptchaEnabled"]);
                if (!captchaEnabled)
                    return false;

                if (_webConfiguration.AllowOverrideParameter)
                {
                    var ignoreCaptcha = IgnoreCaptcha(_httpContextProvider.GetCurrentContext());

                    return !ignoreCaptcha;
                }

                return true;
            }
        }

        public string GoogleCaptchaPrivateKey => ConfigurationManager.AppSettings["Google.CaptchaPrivateKey"];
        public string GoogleCaptchaBaseUrl => ConfigurationManager.AppSettings["Google.CaptchaBaseUrl"];

        private bool IgnoreCaptcha(HttpContextBase context)
        {
            var keyValuePairs = QueryStringParams(context);
            return keyValuePairs[IgnoreRecaptchaParamName] != null;
        }

        private NameValueCollection QueryStringParams(HttpContextBase context)
        {
            var urlParams = HttpUtility.ParseQueryString(
                context.With(ctx => ctx.Request)
                    .With(req => req.Url)
                    .With(url => url.Query)
                    .CheckNullWithDefault(""));

            var urlRefererParams = HttpUtility.ParseQueryString(
                context.With(ctx => ctx.Request)
                    .With(req => req.UrlReferrer)
                    .With(url => url.Query)
                    .CheckNullWithDefault(""));

            var returnParams = new NameValueCollection(urlParams);
            foreach (var key in urlRefererParams.AllKeys)
            {
                if (!returnParams.AllKeys.Contains(key))
                {
                    returnParams.Add(key, urlRefererParams[key]);
                }
            }

            return returnParams;
        }
    }
}
using System.Web;

namespace TAL.QuoteAndApply.Infrastructure.Url
{
    public interface IUrlUtilities
    {
        string UrlEncode(string url);
    }

    public class UrlUtilities : IUrlUtilities
    {
        public string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }
    }
}

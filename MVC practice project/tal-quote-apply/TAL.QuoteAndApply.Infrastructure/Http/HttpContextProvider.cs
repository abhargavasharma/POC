using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TAL.QuoteAndApply.Infrastructure.Http
{
    public interface IHttpContextProvider
    {
        HttpContextBase GetCurrentContext();
    }

    public class HttpContextProvider : IHttpContextProvider
    {
        public HttpContextBase GetCurrentContext()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}

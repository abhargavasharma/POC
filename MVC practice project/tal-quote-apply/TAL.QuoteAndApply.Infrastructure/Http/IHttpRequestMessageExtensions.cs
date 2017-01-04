using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Infrastructure.Http
{
    public interface IIHttpRequestMessageSerializer
    {
        string SerializeJson<T>(object result);
        string SerializeXml<T>(object result);
    }

    public class HttpRequestMessageSerializer : IIHttpRequestMessageSerializer
    {

        public string SerializeJson<T>(object result)
        {
            return JsonConvert.SerializeObject(result);
        }

        public string SerializeXml<T>(object result)
        {
            return SerializeXml(result);
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        public static string SerializeXml(object request)
        {
            return request.ToXml();
        }
    }
}

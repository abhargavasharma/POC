using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TAL.QuoteAndApply.Infrastructure.Concurrency;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;

namespace TAL.QuoteAndApply.Infrastructure.Http
{
    public interface IHttpResponseMessageSerializer
    {
        TResult DeserializeJson<TResult>(HttpResponseMessage response);
        TResult DeserializeXml<TResult>(HttpResponseMessage response);
    }

    public class HttpResponseMessageSerializer : IHttpResponseMessageSerializer
    {
        public TResult DeserializeJson<TResult>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                throw new ApiNotAvailableException("Service is not available");

            if (!response.IsSuccessStatusCode)
                throw ApiException.FromResponse(response);

            var result = JsonConvert.DeserializeObject<TResult>(response.Content.ReadAsStringAsync().Result);

            var token = result as IConcurrencyToken;
            if (token != null)
            {
                token.ConcurrencyToken = GetETag(response.Headers);
            }

            return result;
        }

        public TResult DeserializeXml<TResult>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                throw new ApiNotAvailableException("Service is not available");

            if (!response.IsSuccessStatusCode)
                throw ApiException.FromResponse(response);

            var serializer = new XmlSerializer(typeof(TResult));

            using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
            {
                return (TResult)serializer.Deserialize(reader);
            }
        }

        private string GetETag(HttpResponseHeaders headers)
        {
            return headers.ETag.Tag.Trim("\"".ToCharArray());
        }
    }
}

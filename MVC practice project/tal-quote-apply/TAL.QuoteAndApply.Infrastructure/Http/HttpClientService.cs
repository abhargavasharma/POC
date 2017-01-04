using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Infrastructure.Http
{
    public interface IHttpClientService
    {
        Task<TResult> PostAsync<TResult>(PutOrPostRequest putOrPostRequest);
        Task<HttpResponseMessage> PostAsync(PutOrPostRequest putOrPostRequest);
        Task<TResult> PutAsync<TResult>(PutOrPostRequest putOrPostRequest);
        Task<HttpResponseMessage> PutAsync(PutOrPostRequest putOrPostRequest);
        Task<TResult> GetAsync<TResult>(GetRequest getRequest);
        Task<HttpResponseMessage> GetAsync(GetRequest getRequest);
    }

    public class HttpClientService : IHttpClientService
    {
        private readonly IHttpResponseMessageSerializer _httpResponseMessageSerializer;
        private readonly IIHttpRequestMessageSerializer _httpRequestMessageSerializer;
        private readonly IMimeTypeProvider _mimeTypeProvider;

        public HttpClientService(IHttpResponseMessageSerializer httpResponseMessageSerializer,
            IMimeTypeProvider mimeTypeProvider, IIHttpRequestMessageSerializer httpRequestMessageSerializer)
        {
            _httpResponseMessageSerializer = httpResponseMessageSerializer;
            _mimeTypeProvider = mimeTypeProvider;
            _httpRequestMessageSerializer = httpRequestMessageSerializer;
        }

        public async Task<HttpResponseMessage> PutAsync(PutOrPostRequest putOrPostRequest)
        {
            using (var handler = new HttpClientHandler { Credentials = putOrPostRequest.Credentials })
            {
                using (var client = new HttpClient(handler))
                {
                    if (putOrPostRequest.Timeout.HasValue)
                    {
                        client.Timeout = new TimeSpan(0, 0, putOrPostRequest.Timeout.Value);
                    }

                    var content = new StringContent(SerializeRequest(putOrPostRequest.Data, putOrPostRequest.SourceType), Encoding.UTF8,
                        _mimeTypeProvider.Get(putOrPostRequest.SourceType));

                    var requestMessage = new HttpRequestMessage(HttpMethod.Put, putOrPostRequest.Uri) { Content = content };

                    if (!string.IsNullOrEmpty(putOrPostRequest.Etag))
                        AddIfMatch(requestMessage.Headers, putOrPostRequest.Etag);

                    AddHeaders(requestMessage.Headers, putOrPostRequest.Headers);

                    var result = await client.SendAsync(requestMessage).ConfigureAwait(false);
                    return result;
                }
            }
        }

        public async Task<TResult> GetAsync<TResult>(GetRequest getRequest)
        {
            var response = await GetAsync(getRequest).ConfigureAwait(false);
            return DeserializeResult<TResult>(response, getRequest.SourceType);
        }

        public async Task<HttpResponseMessage> GetAsync(GetRequest getRequest)
        {
            using (var handler = new HttpClientHandler { Credentials = getRequest.Credentials })
            {
                using (var client = new HttpClient(handler))
                {
                    if (getRequest.Timeout.HasValue)
                    {
                        client.Timeout = new TimeSpan(0, 0, getRequest.Timeout.Value);
                    }

                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, getRequest.Uri);

                    if (!string.IsNullOrEmpty(getRequest.Etag))
                        AddIfMatch(requestMessage.Headers, getRequest.Etag);

                    AddHeaders(requestMessage.Headers, getRequest.Headers);

                    var result = await client.SendAsync(requestMessage).ConfigureAwait(false);
                    return result;
                }
            }
        }

        public async Task<TResult> PostAsync<TResult>(PutOrPostRequest putOrPostRequest)
        {
            var result = await PostAsync(putOrPostRequest).ConfigureAwait(false);
            return DeserializeResult<TResult>(result, putOrPostRequest.SourceType);
        }

        public async Task<HttpResponseMessage> PostAsync(PutOrPostRequest putOrPostRequest)
        {
            using (var handler = new HttpClientHandler { Credentials = putOrPostRequest.Credentials })
            {
                if (putOrPostRequest.WebProxy != null)
                    handler.Proxy = putOrPostRequest.WebProxy;

                using (var client = new HttpClient(handler))
                {
                    if (putOrPostRequest.Timeout.HasValue)
                    {
                        client.Timeout = new TimeSpan(0, 0, putOrPostRequest.Timeout.Value);
                    }

                    StringContent content;

                    if (putOrPostRequest.Data == null)
                    {
                        content = new StringContent(string.Empty, Encoding.UTF8, _mimeTypeProvider.Get(putOrPostRequest.SourceType));
                    }
                    else
                    {
                        content = new StringContent(
                            SerializeRequest(putOrPostRequest.Data, putOrPostRequest.SourceType), Encoding.UTF8,
                            _mimeTypeProvider.Get(putOrPostRequest.SourceType));
                    }

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, putOrPostRequest.Uri) { Content = content };

                    if (!string.IsNullOrEmpty(putOrPostRequest.Etag))
                        AddIfMatch(requestMessage.Headers, putOrPostRequest.Etag);

                    AddHeaders(requestMessage.Headers, putOrPostRequest.Headers);

                    var result = await client.SendAsync(requestMessage).ConfigureAwait(false);
                    return result;
                }
            }
        }

        private void AddHeaders(HttpRequestHeaders requestHeaders, IDictionary<string, string> additionalHeaders)
        {
            foreach (var h in additionalHeaders)
            {
                requestHeaders.Add(h.Key, h.Value);
            }
        }

        public async Task<TResult> PutAsync<TResult>(PutOrPostRequest putOrPostRequest)
        {
            var result = await PutAsync(putOrPostRequest).ConfigureAwait(false);
            return DeserializeResult<TResult>(result, putOrPostRequest.SourceType);
        }

        private TResult DeserializeResult<TResult>(HttpResponseMessage result, SourceType sourceType)
        {

            return (sourceType == SourceType.Json)
                       ? _httpResponseMessageSerializer.DeserializeJson<TResult>(result)
                        : _httpResponseMessageSerializer.DeserializeXml<TResult>(result);

        }

        private string SerializeRequest(object request, SourceType sourceType)
        {
            return (sourceType == SourceType.Json)
                       ? _httpRequestMessageSerializer.SerializeJson<object>(request)
                        : _httpRequestMessageSerializer.SerializeXml<object>(request);
        }

        private static void AddIfMatch(HttpRequestHeaders requestHeaders, string etag)
        {
            requestHeaders.IfMatch.Add(new EntityTagHeaderValue("\"" + etag + "\""));
        }
    }
}

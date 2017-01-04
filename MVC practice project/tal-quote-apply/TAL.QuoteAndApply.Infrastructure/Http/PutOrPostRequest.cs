using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace TAL.QuoteAndApply.Infrastructure.Http
{
    public class PutOrPostRequest
    {
        public Uri Uri { get; }
        public object Data { get; }
        public string Etag { get; private set; }
        public SourceType SourceType { get; private set; }
        public ICredentials Credentials { get; private set; }
        public  IDictionary<string, string> Headers { get; }
        public int? Timeout { get; private set; }
        public WebProxy WebProxy { get; private set; }

        public PutOrPostRequest(Uri uri, object data)
        {
            Uri = uri;
            Data = data;
            Etag = null;
            SourceType = SourceType.Json;
            Credentials = null;
            Headers = new Dictionary<string, string>();
        }

        public PutOrPostRequest WithEtag(string etag)
        {
            Etag = etag;
            return this;
        }

        public PutOrPostRequest WithSourceType(SourceType sourceType)
        {
            SourceType = sourceType;
            return this;
        }

        public PutOrPostRequest WithCredentials(ICredentials credentials)
        {
            Credentials = credentials;
            return this;
        }

        public PutOrPostRequest WithProxy(WebProxy proxy)
        {
            WebProxy = proxy;
            return this;
        }
        
        public PutOrPostRequest WithHeader(string name, string value)
        {
            Headers.Add(name, value);
            return this;
        }

        public PutOrPostRequest WithTimeout(int? timeout)
        {
            Timeout = timeout;
            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;

namespace TAL.QuoteAndApply.Infrastructure.Http
{
    public class GetRequest
    {
        public Uri Uri { get; }
        public string Etag { get; private set; }
        public SourceType SourceType { get; private set; }
        public ICredentials Credentials { get; private set; }
        public IDictionary<string, string> Headers { get; }
        public int? Timeout { get; set; }

        public GetRequest(Uri uri)
        {
            Uri = uri;
            Etag = null;
            SourceType = SourceType.Json;
            Credentials = null;
            Headers = new Dictionary<string, string>();
        }

        public GetRequest WithEtag(string etag)
        {
            Etag = etag;
            return this;
        }

        public GetRequest WithSourceType(SourceType sourceType)
        {
            SourceType = sourceType;
            return this;
        }

        public GetRequest WithCredentials(ICredentials credentials)
        {
            Credentials = credentials;
            return this;
        }

        public GetRequest WithHeader(string name, string value)
        {
            Headers.Add(name, value);
            return this;
        }

        public GetRequest WithTimeout(int? timeout)
        {
            Timeout = timeout;
            return this;
        }
    }
}
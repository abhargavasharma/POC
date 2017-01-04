using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TAL.QuoteAndApply.Infrastructure.Http.Exceptions
{
    public class ApiException : Exception
    {
        public HttpResponseMessage Response { get; set; }

        public ApiException(HttpResponseMessage response, string message)
            : base(message)
        {
            this.Response = response;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return this.Response.StatusCode;
            }
        }

        public IEnumerable<string> Errors
        {
            get
            {
                return this.Data.Values.Cast<string>().ToList();
            }
        }

        public static ApiException FromResponse(HttpResponseMessage response)
        {
            var ex = new ApiException(response, string.Format("{0} - {1}", (int)response.StatusCode, response.ReasonPhrase));

            var httpErrorObject = response.Content.ReadAsStringAsync().Result;

            if (!IsJson(httpErrorObject))
            {
                return ex;
            }

            var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };
            var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

            // Sometimes, there may be Model Errors:
            if (deserializedErrorObject != null && deserializedErrorObject.ModelState != null)
            {
                var errors = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value)).ToList();

                for (int i = 0; i < errors.Count; i++)
                {
                    // Wrap the errors up into the base Exception.Data Dictionary:
                    ex.Data.Add(i, errors.ElementAt(i));
                }
            }
            else if (!string.IsNullOrWhiteSpace(httpErrorObject))
            {
                // No model errors - so just get the stuff out
                var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);
                foreach (var kvp in error)
                {
                    // Wrap the errors up into the base Exception.Data Dictionary:
                    ex.Data.Add(kvp.Key, kvp.Value);
                }
            }

            return ex;
        }

        private static bool IsJson(string content)
        {
            try
            {
                JToken.Parse(content);
                return true;
            }
            catch (JsonReaderException jsonReaderException)
            {
                Trace.WriteLine(jsonReaderException);
                return false;
            }
        }
    }
}
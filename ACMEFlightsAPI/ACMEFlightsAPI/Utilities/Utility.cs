using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace ACMEFlightsAPI.Utilities
{
    public static class Utility
    {
        public static HttpResponseMessage CreateManualResponseFromObject(object value, HttpStatusCode code)
        {
            return new HttpResponseMessage
            {
                StatusCode = code,
                Content = new StringContent(
                         Newtonsoft.Json.JsonConvert.SerializeObject(value), System.Text.Encoding.UTF8,
                        "application/json"),
            };
        }
    }
}
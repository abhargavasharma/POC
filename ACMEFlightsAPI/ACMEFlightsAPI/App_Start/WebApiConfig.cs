using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Net.Http.Headers;
using ACMEFlightsAPI.Attributes;

namespace ACMEFlightsAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
			
			config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

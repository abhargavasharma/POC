using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TAL.QuoteAndApply.Infrastructure.Serialization
{
    public class JsonSerializerSettingsProvider
    {
        public static JsonSerializerSettings GetSettings()
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;

            return settings;
        }
    }
}

using System.Xml.Serialization;
using Newtonsoft.Json;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Serialization;

namespace TAL.QuoteAndApply.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static string SerializeToPascalCase(object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, JsonSerializerSettingsProvider.GetSettings());
        }

        public static string ToJson(this object data)
        {
            return SerializeToPascalCase(data);
        }

        public static string ToXml(this object data)
        {
            var stringwriter = new HttpRequestMessageSerializer.Utf8StringWriter();
            var serializer = new XmlSerializer(data.GetType());
            serializer.Serialize(stringwriter, data);
            return stringwriter.ToString();
        }
    }
}

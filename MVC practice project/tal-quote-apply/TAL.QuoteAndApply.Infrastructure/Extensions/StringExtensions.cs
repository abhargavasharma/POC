using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Serialization;

namespace TAL.QuoteAndApply.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string propName, char divider = ' ')
        {
            var array = propName.Split(divider);
            var camelCaseList = new string[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                var prop = array[i];
                if (prop.Length > 0)
                    camelCaseList[i] = prop.Substring(0, 1).ToLower() + prop.Substring(1, prop.Length - 1);
            }
            return String.Join(divider.ToString(), camelCaseList);
        }

        public static string ToWords(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(char.ToUpper(text[0]));
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static string ToTitleCase(this string propName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(propName);
        }

        public static DateTime? ToDateExcactDdMmYyyy(this string str)
        {
            DateTime date;
            var validDate = DateTime.TryParseExact(str, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

            if (validDate)
            {
                return date;
            }

            return null;
        }

        public static T FromXml<T>(this string content)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var reader = new StringReader(content))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static T FromJson<T>(this string content)
        {
            return JsonConvert.DeserializeObject<T>(content, JsonSerializerSettingsProvider.GetSettings());
		}

        public static object FromJson(this string content, Type type)
        {
            return JsonConvert.DeserializeObject(content, type, JsonSerializerSettingsProvider.GetSettings());
        }

        public static string EmptyOrWhiteSpaceToNull(this string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            return content;
        }

        public static string EmptyOrWhiteSpaceToNullAndStripSpaces(this string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            return content.Replace(" ", "");
        }

        public static bool IsNullOrEmpty(this string str)
        {
           return string.IsNullOrEmpty(str);
        }
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string ToBase64String(this byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public static long ToLong(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return long.MinValue;

            //Parse to double first incase string has decimal point
            return (long) double.Parse(str);
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

    }
}

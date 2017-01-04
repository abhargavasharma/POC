using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Security.AccessControl;

namespace TAL.QuoteAndApply.Infrastructure.Resource
{
    public interface IResourceFileReader
    {
        string GetContentsOfResource(Assembly assembly, string resourceName);
    }

    public class ResourceFileReader : IResourceFileReader
    {

        public string GetContentsOfResource(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                {
                    throw new FileNotFoundException($"could not find resource {resourceName}");
                }
            }
        }
    }
}
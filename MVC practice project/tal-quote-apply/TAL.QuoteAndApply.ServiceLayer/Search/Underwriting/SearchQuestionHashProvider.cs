using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Underwriting
{
    public static class SearchQuestionHashProvider
    {
        private static readonly IDictionary<string, string> HashLookup;
        private static readonly object PadLock;
        static SearchQuestionHashProvider()
        {
            HashLookup = new Dictionary<string, string>();
            PadLock = new object();
        }

        public static string CreateHashKeyFor(string identityKey)
        {
            // TODO: only hash questions that need to be indexed
            
            if (HashLookup.ContainsKey(identityKey))
                return HashLookup[identityKey];

            lock (PadLock)
            {
                if (HashLookup.ContainsKey(identityKey))
                    return HashLookup[identityKey];

                using (var md5Hash = MD5.Create())
                {
                    var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(identityKey));
                    var sBuilder = new StringBuilder();
                    foreach (byte b in data)
                    {
                        sBuilder.Append(b.ToString("x2"));
                    }
                    return sBuilder.ToString();
                }
            }
        }
    }
}

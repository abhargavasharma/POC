using System;
using System.Text;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class SimpleUri
    {
        public string Path { get; set; }
        public string Query { get; set; }

        public SimpleUri(string pathAndQuery)
        {
            var seg = pathAndQuery.Split(new[] {'?'});
            if (seg.Length == 2)
            {
                Path = seg[0];
                Query = seg[1];
            }
            else if (seg.Length == 1)
            {
                Path = seg[0];
            }
            else
            {
                throw new Exception("Invalid path and query value. Must contain either 1 or no '?' characters");
            }
  
        }

        public SimpleUri(string path, string query)
        {
            Path = path;
            Query = query;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Path);
            if (!string.IsNullOrEmpty(Query))
            {
                sb.Append("?").Append(Query);
            }
            return sb.ToString();
        }
    }
}
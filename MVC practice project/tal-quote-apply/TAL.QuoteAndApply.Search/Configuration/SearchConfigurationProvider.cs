using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Search.Configuration
{
    public interface ISearchConfigurationProvider
    {
        string ConnectionString { get; }
        string LuceneIndexRootPath { get; }
        string SearchDatabase { get; }
    }

    public class SearchConfigurationProvider : ISearchConfigurationProvider
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["Search.SqlConnectionString"]; }
        }

        public string LuceneIndexRootPath
        {
            get { return ConfigurationManager.AppSettings["Search.IndexRoot"]; }
        }

        public string SearchDatabase
        {
            get { return ConfigurationManager.AppSettings["Search.Database"]; }
        }
    }
}

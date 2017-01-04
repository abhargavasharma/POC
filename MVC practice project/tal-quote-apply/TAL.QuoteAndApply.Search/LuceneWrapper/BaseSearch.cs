using System.IO;
using Lucene.Net.Store;

namespace TAL.QuoteAndApply.Search.LuceneWrapper
{
    public enum LuceneDataProvider
    {
        Mongo,
        SqlServer
    }

    public class LuceneParameters
    {
        public string RootDirectory { get; private set; }
        public string IndexName { get; private set; }
        public string MongoConnectionString { get; private set; }
        public string SqlConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public string DocumentSearchField { get; private set; }
        public LuceneDataProvider DataProvider { get; private set; }

        private LuceneParameters()
        {

        }

        public static LuceneParameters ForMongo(string rootDirectory, string indexName, string mongoConnectionString, string databaseName, string documentSearchField = "Similies")
        {
            return new LuceneParameters
            {
                RootDirectory = rootDirectory,
                IndexName = indexName,
                MongoConnectionString = mongoConnectionString,
                DocumentSearchField = documentSearchField,
                DatabaseName = databaseName,
                DataProvider = LuceneDataProvider.Mongo
            };
        }

        public static LuceneParameters ForSql(string rootDirectory, string indexName, string sqlConnectionString, string databaseName, string documentSearchField = "Similies")
        {
            return new LuceneParameters
            {
                RootDirectory = rootDirectory,
                IndexName = indexName,
                SqlConnectionString = sqlConnectionString,
                DocumentSearchField = documentSearchField,
                DatabaseName = databaseName,
                DataProvider = LuceneDataProvider.SqlServer
            };
        }
    }

    /// <summary>
    /// The abstract base class to be implemented by everything that uses the Lucene directory
    /// </summary>
    public abstract class BaseSearch<T>
    {
        private readonly FSDirectory _luceneDirectory;


        /// <summary>
        /// The App Data folder - or the folder where the lucene folder is placed under as FSDirectory object
        /// </summary>
        public FSDirectory LuceneDirectory
        {
            get { return _luceneDirectory; }
        }

        protected BaseSearch(LuceneParameters parameters)
        {
            var path = Path.Combine(parameters.RootDirectory, parameters.IndexName);
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            _luceneDirectory = new SimpleFSDirectory(directoryInfo, new SimpleFSLockFactory());
        }
    }
}

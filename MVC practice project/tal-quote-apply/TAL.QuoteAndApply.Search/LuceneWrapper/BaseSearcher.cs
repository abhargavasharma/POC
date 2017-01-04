using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Version = Lucene.Net.Util.Version;

namespace TAL.QuoteAndApply.Search.LuceneWrapper
{
    /// <summary>
    /// Base abstract class where that every Searcher should implmement
    /// </summary>
    public abstract class BaseSearcher<T> : BaseSearch<T>, IDisposable
    {
        private const int HitsLimit = 1000;
        private readonly IndexSearcher _indexSearcher;
        private readonly Analyzer _analyzer;
        private readonly LuceneParameters _parameters;

        protected BaseSearcher(LuceneParameters parameters) : base(parameters)
        {
            _parameters = parameters;
            _indexSearcher = new IndexSearcher(LuceneDirectory);
            _analyzer = new WhitespaceAnalyzer();
        }

        protected SearchResult Search(string searchQuery)
        {
            var searchResults = new SearchResult
            {
                SearchTerm = searchQuery,
                SearchResultItems = new List<SearchResultItem>()
            };

            var parser = new QueryParser(Version.LUCENE_29, _parameters.DocumentSearchField, _analyzer);
            var query = ParseQuery(searchQuery, parser);
            var hits = _indexSearcher.Search(query, HitsLimit).ScoreDocs;
            
            if (hits != null)
            {
                searchResults.Hits = hits.Count();
                foreach (var hit in hits)
                {
                    var doc = _indexSearcher.Doc(hit.Doc);
                    searchResults.SearchResultItems.Add(new SearchResultItem
                    {
                        Id = doc.Get("Id"),
                        Score = hit.Score
                    });
                }
            }
            
            return searchResults;
        }

        /// <summary>
        /// Parse the givven query string to a Lucene Query object
        /// </summary>
        /// <param name="searchQuery">The query string</param>
        /// <param name="parser">The Lucense QueryParser</param>
        /// <returns>A Lucene Query object</returns>
        private Query ParseQuery(string searchQuery, QueryParser parser)
        {
            parser.AllowLeadingWildcard = true;
            Query q;
            try
            {
                q = parser.Parse(searchQuery);
            }

            catch (ParseException e)
            {
                Debug.WriteLine("Query parser exception", e);
                q = null;
            }

            if (q == null || string.IsNullOrEmpty(q.ToString()))
            {
                var cooked = Regex.Replace(searchQuery, @"[^\w\.@-]", " ");
                q = parser.Parse(cooked);
            }

            return q;
        }


        public void Dispose()
        {
            _analyzer.Close();
            _indexSearcher.Dispose();
        }
    }
}

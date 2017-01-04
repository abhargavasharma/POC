using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace TAL.QuoteAndApply.Search.LuceneWrapper
{
    /// <summary>
    /// Base abstract class that every Writer should implement
    /// </summary>
    public abstract class BaseWriter<T> : BaseSearch<T>
    {
        protected BaseWriter(LuceneParameters parameters) : base(parameters)
        {
        }

        /// <summary>
        /// Private helper to add an item to the Index
        /// </summary>
        /// <param name="doc">A ADocument type, representing the values that have to be added to the index</param>
        /// <param name="writer">The Lucene writer</param>
        private void AddItemToIndex(ADocument doc, IndexWriter writer)
        {
            var query = new TermQuery(new Term("Id", doc.Id.ToString()));
            writer.DeleteDocuments(query);
            writer.AddDocument(doc.Document);
        }

        /// <summary>
        /// Adds or update items in the Lucene index
        /// </summary>
        /// <param name="docs">The documents that have to be updated or added in the database</param>
        protected void AddUpdateItemsToIndex(IEnumerable<ADocument> docs)
        {
            var standardAnalyzer = new WhitespaceAnalyzer();

            using (var writer = new IndexWriter(LuceneDirectory, standardAnalyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var doc in docs)
                {
                    AddItemToIndex(doc, writer);
                }
                standardAnalyzer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Private helper to delete an item from the index
        /// </summary>
        /// <param name="doc">The document representing the item that has to be deleted</param>
        /// <param name="writer">The Lucene writer</param>
        private void DeleteItemFromIndex(ADocument doc, IndexWriter writer)
        {
            var query = new TermQuery(new Term("Id", doc.Id.ToString()));
            writer.DeleteDocuments(query);
        }

        /// <summary>
        /// Deletes ites from the Lucene index
        /// </summary>
        /// <param name="docs"></param>
        protected void DeleteItemsFromIndex(IEnumerable<ADocument> docs)
        {
            var standardAnalyzer = new WhitespaceAnalyzer();

            using (var writer = new IndexWriter(LuceneDirectory, standardAnalyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var doc in docs)
                {
                    DeleteItemFromIndex(doc, writer);
                }
                standardAnalyzer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// optimizes the Lucene Index
        /// </summary>
        protected void Optimize()
        {
            var standardAnalyzer = new WhitespaceAnalyzer();

            using (var writer = new IndexWriter(LuceneDirectory, standardAnalyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                standardAnalyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }
    }
}

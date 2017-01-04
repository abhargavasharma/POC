using System;
using Lucene.Net.Index;

namespace TAL.QuoteAndApply.Search.LuceneWrapper
{
    public abstract class BaseReader<T> : BaseSearch<T>
    {
        protected BaseReader(LuceneParameters parameters) : base(parameters)
        {
        }

        protected int GetNumberOfDocuments()
        {
            try
            {
                using (var reader = IndexReader.Open(LuceneDirectory, true))
                {
                    return reader.NumDocs();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
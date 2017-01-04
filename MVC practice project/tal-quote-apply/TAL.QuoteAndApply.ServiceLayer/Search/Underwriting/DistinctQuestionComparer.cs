using System.Collections.Generic;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Underwriting
{
    public class DistinctQuestionComparer : IEqualityComparer<ReadOnlyQuestion>
    {
        public static DistinctQuestionComparer Instance { get; private set; }

        static DistinctQuestionComparer()
        {
            Instance = new DistinctQuestionComparer();
        }

        private DistinctQuestionComparer()
        {
            
        }

        public bool Equals(ReadOnlyQuestion x, ReadOnlyQuestion y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ReadOnlyQuestion obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
using System.Collections.Generic;

namespace TAL.QuoteAndApply.Product.Models.Types
{
    public class SelectedCovers : List<string>
    {
        public SelectedCovers(IEnumerable<string> array) : base(array)
        {
        }

        public static implicit operator SelectedCovers(string[] array)
        {
            return new SelectedCovers(array);
        }
    }
}
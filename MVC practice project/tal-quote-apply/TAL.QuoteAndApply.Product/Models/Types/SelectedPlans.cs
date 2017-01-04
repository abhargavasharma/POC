using System.Collections.Generic;

namespace TAL.QuoteAndApply.Product.Models.Types
{
    public class SelectedPlans : List<string>
    {
        public SelectedPlans(IEnumerable<string> array) : base(array)
        {
        }

        public static implicit operator SelectedPlans(string[] array)
        {
            return new SelectedPlans(array);
        }
    }
}

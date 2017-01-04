using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class ReadOnlyTotalLoadings
    {
        public ReadOnlyTotalLoadings(TotalLoadings loadings)
        {
            Variable = loadings.Variable;
            Fixed = loadings.Fixed;
            PerMille = loadings.PerMille;
        }

        public decimal Variable { get; private set; }
        public decimal Fixed { get; private set; }
        public decimal PerMille { get; private set; }

    }
}
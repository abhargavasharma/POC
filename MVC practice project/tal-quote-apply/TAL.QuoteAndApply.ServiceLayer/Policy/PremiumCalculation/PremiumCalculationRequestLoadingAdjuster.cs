using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation
{
    public interface IPremiumCalculationRequestLoadingAdjuster
    {
        Loadings GetAdjustedLoadings(Loadings totalLoadings, string coverCode);
    }

    public class NullPremiumCalcLoadingAdjuster : IPremiumCalculationRequestLoadingAdjuster
    {
        public Loadings GetAdjustedLoadings(Loadings totalLoadings, string coverCode)
        {
            return totalLoadings;
        }
    }

    public class PremiumCalcLoadingAdjuster : IPremiumCalculationRequestLoadingAdjuster
    {
        private readonly UnderwritingLoading _adjustByLoading;
        public IEnumerable<string> CoverCodes { get; }

        public PremiumCalcLoadingAdjuster(UnderwritingLoading adjustByLoading, IEnumerable<string> coverCodes)
        {
            _adjustByLoading = adjustByLoading;
            CoverCodes = coverCodes;
        }

        public static PremiumCalcLoadingAdjuster ReduceBy(UnderwritingLoading adjustByLoading,
            IEnumerable<string> coverCodes)
        {
            var reducedLoading = new UnderwritingLoading(adjustByLoading.Name, adjustByLoading.LoadingType, -adjustByLoading.Amount);
            return new PremiumCalcLoadingAdjuster(reducedLoading, coverCodes);
        }

        public Loadings GetAdjustedLoadings(Loadings totalLoadings, string coverCode)
        {
            var percentageLoadingAdjustment = 0M;
            var perMilleAdjustment = 0M;

            if (CoverCodes.Contains(coverCode))
            {
                percentageLoadingAdjustment = _adjustByLoading.LoadingType == LoadingType.Variable
                    ? _adjustByLoading.Amount
                    : 0;
                perMilleAdjustment = _adjustByLoading.LoadingType == LoadingType.PerMille ? _adjustByLoading.Amount : 0;
            }

            return new Loadings(
                totalLoadings.PercentageLoading + percentageLoadingAdjustment,
                totalLoadings.PerMilleLoading + perMilleAdjustment
                );
        }
    }
}

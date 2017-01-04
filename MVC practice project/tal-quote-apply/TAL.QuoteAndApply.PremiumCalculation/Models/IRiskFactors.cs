using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public interface IRiskFactors
    {
        int Age { get; }
        Gender Gender { get; }
        bool Smoker { get; }
        string OccupationClass { get; }
    }
}
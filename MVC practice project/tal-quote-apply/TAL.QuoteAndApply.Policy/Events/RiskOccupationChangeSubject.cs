using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface IRiskOccupationChangeSubject : ISubject<IRiskOccupationChangeObserver>
    { }

    public class RiskOccupationChangeSubject : BaseSubject<IRiskOccupationChangeObserver, IOccupationRatingFactors>, IRiskOccupationChangeSubject
    {
    }
}
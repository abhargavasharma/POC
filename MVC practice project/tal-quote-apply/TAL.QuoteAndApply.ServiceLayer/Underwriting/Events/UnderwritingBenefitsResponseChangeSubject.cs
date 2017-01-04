using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Underwriting.Models.Event;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Events
{
    public interface IUnderwritingBenefitsResponseChangeSubject : ISubject<IUnderwritingBenefitsResponseChangeObserver>
    { }

    public class UnderwritingBenefitsResponseChangeSubject : BaseSubject<IUnderwritingBenefitsResponseChangeObserver, UnderwritingBenefitResponsesChangeParam>, IUnderwritingBenefitsResponseChangeSubject
    {
    }
}

using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface IRiskChangeSubject : ISubject<IRiskChangeObserver>
    { }

    public class RiskChangeSubject : BaseSubject<IRiskChangeObserver, IRisk>, IRiskChangeSubject
    {
    }
}
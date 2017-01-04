using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface IPlanChangeSubject : ISubject<IPlanChangeObserver>
    { }


    public class PlanChangeSubject : BaseSubject<IPlanChangeObserver, IPlan>, IPlanChangeSubject
    {
    }
}
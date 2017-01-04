using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events
{
    public interface ISmokerStatusChangeSubject : ISubject<ISmokerStatusChangeObserver>
    {
    }

    public class SmokerStatusChangeSubject : BaseSubject<ISmokerStatusChangeObserver, UpdateSmokerStatusParam>, ISmokerStatusChangeSubject
    {
    }
}
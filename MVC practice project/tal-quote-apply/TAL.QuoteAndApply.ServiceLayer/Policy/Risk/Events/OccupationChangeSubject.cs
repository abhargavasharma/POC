using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events
{
    public interface IOccupationChangeSubject : ISubject<IOccupationChangeObserver>
    {
    }

    public class OccupationChangeSubject : BaseSubject<IOccupationChangeObserver, UpdateOccupationParam>, IOccupationChangeSubject
    {
    }

}

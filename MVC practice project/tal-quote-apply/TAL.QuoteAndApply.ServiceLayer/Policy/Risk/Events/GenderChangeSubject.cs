using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events
{
    public interface IGenderChangeSubject : ISubject<IGenderChangeObserver>
    { }

    public class GenderChangeSubject : BaseSubject<IGenderChangeObserver, UpdateGenderParam>, IGenderChangeSubject
    { }

}

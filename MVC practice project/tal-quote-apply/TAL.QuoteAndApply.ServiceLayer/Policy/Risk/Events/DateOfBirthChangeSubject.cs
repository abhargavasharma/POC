using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events
{
    public interface IDateOfBirthChangeSubject : ISubject<IDateOfBirthChangeObserver>
    { }

    public class DateOfBirthChangeSubject : BaseSubject<IDateOfBirthChangeObserver, UpdateDateOfBirthParam>, IDateOfBirthChangeSubject
    { }
}

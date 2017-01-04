using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Data;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface IOptionChangeSubject : ISubject<IOptionChangeObserver>
    { }

    public class OptionChangeSubject : BaseSubject<IOptionChangeObserver, IOption>, IOptionChangeSubject
    {
    }
}
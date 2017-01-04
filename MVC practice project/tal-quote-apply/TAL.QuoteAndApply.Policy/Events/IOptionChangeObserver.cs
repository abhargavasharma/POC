using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Data;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface IOptionChangeObserver : ISimpleObserver<IOption>
    { }
}
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface ICoverLoadingChangeSubject : ISubject<ICoverLoadingChangeObserver>
    { }

    public class CoverLoadingChangeSubject : BaseSubject<ICoverLoadingChangeObserver, ICoverLoading>, ICoverLoadingChangeSubject
    {
    }
}
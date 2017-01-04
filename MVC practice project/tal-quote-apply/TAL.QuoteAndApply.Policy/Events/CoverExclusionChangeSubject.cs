using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface ICoverExclusionChangeSubject : ISubject<ICoverExclusionChangeObserver>
    { }

    public class CoverExclusionChangeSubject : BaseSubject<ICoverExclusionChangeObserver, ICoverExclusion>, ICoverExclusionChangeSubject
    {
    }
}

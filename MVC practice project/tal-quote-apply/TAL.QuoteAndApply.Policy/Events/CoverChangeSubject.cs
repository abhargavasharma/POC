using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Events
{
    public interface ICoverChangeSubject : ISubject<ICoverChangeObserver>
    { }

    public class CoverChangeSubject : BaseSubject<ICoverChangeObserver, ICover>, ICoverChangeSubject
    {
    }
}
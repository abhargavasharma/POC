using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events
{
    public interface IResidencyChangeSubject : ISubject<IResidencyChangeObserver>
    { }

    public class ResidencyChangeSubject : BaseSubject<IResidencyChangeObserver, UpdateResidencyParam>, IResidencyChangeSubject
    { }
}
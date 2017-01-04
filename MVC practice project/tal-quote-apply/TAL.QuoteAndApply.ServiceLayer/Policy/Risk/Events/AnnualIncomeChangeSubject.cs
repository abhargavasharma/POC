using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events
{
    public interface IAnnualIncomeChangeSubject : ISubject<IAnnualIncomeChangeObserver>
    { }

    public class AnnualIncomeChangeSubject : BaseSubject<IAnnualIncomeChangeObserver, UpdateAnnualIncomeParam>, IAnnualIncomeChangeSubject
    { }
}

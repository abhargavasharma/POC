using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;

namespace TAL.QuoteAndApply.ServiceLayer.Ioc
{
    public class UnderwritingEventsRegistration : IEventsRegistration
    {
        private readonly IUnderwritingBenefitsResponseChangeSubject _underwritingBenefitsResponseChangeSubject;
        private readonly IEnumerable<IUnderwritingBenefitsResponseChangeObserver> _underwritingBenefitsResponseChangeObservers;

        public UnderwritingEventsRegistration(IUnderwritingBenefitsResponseChangeSubject underwritingBenefitsResponseChangeSubject,
            IEnumerable<IUnderwritingBenefitsResponseChangeObserver> underwritingBenefitsResponseChangeObservers)
        {
            _underwritingBenefitsResponseChangeSubject = underwritingBenefitsResponseChangeSubject;
            _underwritingBenefitsResponseChangeObservers = underwritingBenefitsResponseChangeObservers;
        }

        public void RegisterAllObserversWithSubjects()
        {
            foreach (var o in _underwritingBenefitsResponseChangeObservers)
            {
                _underwritingBenefitsResponseChangeSubject.Subscribe(o);
            }
        }
    }
}
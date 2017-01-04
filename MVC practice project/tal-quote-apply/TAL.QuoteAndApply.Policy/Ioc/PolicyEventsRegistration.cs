using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Events;

namespace TAL.QuoteAndApply.Policy.Ioc
{
    public class PolicyEventsRegistration : IEventsRegistration
    {
        private readonly IEnumerable<ICoverChangeObserver> _coverChangeObservers;
        private readonly ICoverChangeSubject _coverChangeSubject;

        private readonly IEnumerable<IRiskChangeObserver> _riskChangeObservers;
        private readonly IRiskChangeSubject _riskChangeSubject;

        private readonly IEnumerable<IPlanChangeObserver> _planChangeObservers;
        private readonly IPlanChangeSubject _planChangeSubject;

        private readonly IEnumerable<IOptionChangeObserver> _optionChangeObservers;
        private readonly IOptionChangeSubject _optionChangeSubject;

        private readonly IEnumerable<IRiskOccupationChangeObserver> _riskOccupationChangeObservers;
        private readonly IRiskOccupationChangeSubject _riskOccupationChangeSubject;

        public PolicyEventsRegistration(IEnumerable<ICoverChangeObserver> coverChangeObservers,
            ICoverChangeSubject coverChangeSubject, IEnumerable<IRiskChangeObserver> riskChangeObservers,
            IRiskChangeSubject riskChangeSubject, IEnumerable<IPlanChangeObserver> planChangeObservers,
            IPlanChangeSubject planChangeSubject, IEnumerable<IOptionChangeObserver> optionChangeObservers,
            IOptionChangeSubject optionChangeSubject,
            IEnumerable<IRiskOccupationChangeObserver> riskOccupationChangeObservers,
            IRiskOccupationChangeSubject riskOccupationChangeSubject)
        {
            _coverChangeObservers = coverChangeObservers;
            _coverChangeSubject = coverChangeSubject;
            _riskChangeObservers = riskChangeObservers;
            _riskChangeSubject = riskChangeSubject;
            _planChangeObservers = planChangeObservers;
            _planChangeSubject = planChangeSubject;
            _optionChangeObservers = optionChangeObservers;
            _optionChangeSubject = optionChangeSubject;
            _riskOccupationChangeObservers = riskOccupationChangeObservers;
            _riskOccupationChangeSubject = riskOccupationChangeSubject;
        }

        public void RegisterCoverObservers()
        {
            foreach (var o in _coverChangeObservers)
            {
                _coverChangeSubject.Subscribe(o);
            }
        }

        public void RegisterRiskObservers()
        {
            foreach (var o in _riskChangeObservers)
            {
                _riskChangeSubject.Subscribe(o);
            }
        }

        public void RegisterOptionObservers()
        {
            foreach (var o in _optionChangeObservers)
            {
                _optionChangeSubject.Subscribe(o);
            }
        }

        public void RegisterPlanObservers()
        {
            foreach (var o in _planChangeObservers)
            {
                _planChangeSubject.Subscribe(o);
            }
        }

        public void RegisterRiskOccupationObservers()
        {
            foreach (var o in _riskOccupationChangeObservers)
            {
                _riskOccupationChangeSubject.Subscribe(o);
            }
        }

        public void RegisterAllObserversWithSubjects()
        {
            RegisterPlanObservers();
            RegisterOptionObservers();
            RegisterCoverObservers();
            RegisterRiskObservers();
        }
    }
}

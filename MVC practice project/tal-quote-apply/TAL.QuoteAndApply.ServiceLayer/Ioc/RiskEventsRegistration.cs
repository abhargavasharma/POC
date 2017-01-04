using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events;

namespace TAL.QuoteAndApply.ServiceLayer.Ioc
{
    public class RiskEventsRegistration : IEventsRegistration
    {
        private readonly IDateOfBirthChangeSubject _dateOfBirthChangeSubject;
        private readonly IEnumerable<IDateOfBirthChangeObserver> _dateOfBirthObservers;
        private readonly IGenderChangeSubject _genderChangeSubject;
        private readonly IEnumerable<IGenderChangeObserver> _genderObservers;
        private readonly IOccupationChangeSubject _occupationChangeSubject;
        private readonly IEnumerable<IOccupationChangeObserver> _occupationObservers;
        private readonly IResidencyChangeSubject _residencyChangeSubject;
        private readonly IEnumerable<IResidencyChangeObserver> _residencyChangeObservers;
        private readonly ISmokerStatusChangeSubject _smokerStatusChangeSubject;
        private readonly IEnumerable<ISmokerStatusChangeObserver> _smokerStatusChangeObservers;
        private readonly IAnnualIncomeChangeSubject _annualIncomeChangeSubject;
        private readonly IEnumerable<IAnnualIncomeChangeObserver> _annualIncomeChangeObservers;

        public RiskEventsRegistration(IDateOfBirthChangeSubject dateOfBirthChangeSubject, 
            IEnumerable<IDateOfBirthChangeObserver> dateOfBirthObservers,
            IGenderChangeSubject genderChangeSubject,
            IEnumerable<IGenderChangeObserver> genderObservers,
            IOccupationChangeSubject occupationChangeSubject,
            IEnumerable<IOccupationChangeObserver> occupationObservers,
            IResidencyChangeSubject residencyChangeSubject,
            IEnumerable<IResidencyChangeObserver> residencyChangeObservers,
            ISmokerStatusChangeSubject smokerStatusChangeSubject,
            IEnumerable<ISmokerStatusChangeObserver> smokerStatusChangeObservers,
            IAnnualIncomeChangeSubject annualIncomeChangeSubject, 
            IEnumerable<IAnnualIncomeChangeObserver> annualIncomeChangeObservers)
        {
            _dateOfBirthChangeSubject = dateOfBirthChangeSubject;
            _dateOfBirthObservers = dateOfBirthObservers;
            _genderChangeSubject = genderChangeSubject;
            _genderObservers = genderObservers;
            _occupationChangeSubject = occupationChangeSubject;
            _occupationObservers = occupationObservers;
            _residencyChangeSubject = residencyChangeSubject;
            _residencyChangeObservers = residencyChangeObservers;
            _smokerStatusChangeSubject = smokerStatusChangeSubject;
            _smokerStatusChangeObservers = smokerStatusChangeObservers;
            _annualIncomeChangeSubject = annualIncomeChangeSubject;
            _annualIncomeChangeObservers = annualIncomeChangeObservers;
        }

        public void RegisterAllObserversWithSubjects()
        {
            RegisterDateOfBirthObservers();
            RegisterGenderObservers();
            RegisterOccupationObservers();
            RegisterResidencyObservers();
            RegisterSmokerStatusObservers();
            RegisterAnnualIncomeObservers();
        }

        private void RegisterResidencyObservers()
        {
            foreach (var o in _residencyChangeObservers)
            {
                _residencyChangeSubject.Subscribe(o);
            }
        }

        private void RegisterSmokerStatusObservers()
        {
            foreach (var o in _smokerStatusChangeObservers)
            {
                _smokerStatusChangeSubject.Subscribe(o);
            }
        }

        private void RegisterOccupationObservers()
        {
            foreach(var o in _occupationObservers)
            {
                _occupationChangeSubject.Subscribe(o);
            }
        }

        private void RegisterGenderObservers()
        {
            foreach (var o in _genderObservers)
            {
                _genderChangeSubject.Subscribe(o);
            }
        }

        private void RegisterDateOfBirthObservers()
        {
            foreach (var o in _dateOfBirthObservers)
            {
                _dateOfBirthChangeSubject.Subscribe(o);
            }
        }
        private void RegisterAnnualIncomeObservers()
        {
            foreach (var o in _annualIncomeChangeObservers)
            {
                _annualIncomeChangeSubject.Subscribe(o);
            }
        }
    }
}

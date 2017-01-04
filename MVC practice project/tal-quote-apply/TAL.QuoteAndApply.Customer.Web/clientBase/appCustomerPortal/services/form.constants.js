(function (module) {
    'use strict';

    module.constant('FORM', {
        PERSONAL_DETAILS: {
            TITLES: ['Dr','Mr', 'Mrs', 'Miss', 'Ms']
        },
        ADDRESS: {
            STATES: ['ACT', 'NSW', 'NT', 'QLD', 'SA', 'TAS', 'VIC', 'WA'],
            COUNTRIES: ['Australia']
        },
        CONTACT: {
            CONTACT_METHODS: ['Phone', 'SMS', 'Email']
        },
        SMOKER: {
            STATUS: [
                {value: 'HaventSmoked', name: 'I haven\'t smoked at all'},
                {value: 'SmokedLessThan10', name: 'I have smoked less than 10 times'},
                {value: 'Smoked10To20', name: 'I have smoked between 10 and 20 times'},
                {value: 'SmokedMoreThan20', name: 'I have smoked more than 20 times'}
            ]
        }
    });

})(angular.module('appCustomerPortal'));


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
        POLICY: {
            WAITING_PERIODS: [
                {value: 2, name: '2 Weeks' },
                {value: 4, name: '4 Weeks' },
                {value: 13, name: '13 Weeks' },
                {value: 104, name: '104 Weeks' }
            ],
            BENEFIT_PERIODS: [
                {value: 1, name: '1 Year' },
                {value: 2, name: '2 Years' },
                {value: 5, name: '5 Years' }
            ],
            OCCUPATION_DEFINITION_OPTIONS: [
                {value: 'AnyOccupation', name: 'Any Occupation' },
                {value: 'OwnOccupation', name: 'Own Occupation' }
            ]
        },
        COVER_SELECTION: {
            PREMIUM_TYPES: ['Stepped', 'Level']
        },
        REFERRAL: {
            STATES: [
                {name: 'Unresolved', value: 'Unresolved'},
                {name: 'In Progress', value: 'InProgress'},
                {name: 'Resolved', value: 'Resolved'}
            ],
            FILTER: {
                PLAN_TYPES: [
                    {name: 'Life', code: 'DTH', selected: false},
                    {name: 'TPD', code: 'TPS', selected: false},
                    {name: 'Recovery Insurance', code: 'TRS', selected: false},
                    {name: 'Income Protection', code: 'IP', selected: false}
                ],
                STATUSES: [
                    {name: 'Unresolved', code: 'Unresolved', selected: true},
                    {name: 'In Progress', code: 'InProgress', selected: true},
                    {name: 'Resolved', code: 'Resolved', selected: false}
                ]
            }
        },
        CONSENTS: {
            MODEL: {
                expressConsent: false,
                dncs: [
                    {name: 'dncEmail', selected: false, description: 'Email'},
                    {name: 'dncMobile', selected: false, description: 'Mobile'},
                    {name: 'dncHomeNumber', selected: false, description: 'Home Number'},
                    {name: 'dncPostalMail', selected: false, description: 'Postal Mail'}
                ]
            },
            SUBMIT_MODEL: [
                {name: 'expressConsent', selected: false},
                {name: 'dncEmail', selected: false},
                {name: 'dncMobile', selected: false},
                {name: 'dncHomeNumber', selected: false},
                {name: 'dncPostalMail', selected: false}
            ]
        },
        INTERVIEW: {
            ACTIONS: [
                { value: 'ViewReadOnly', name: 'View interview in read only mode.' },
                { value: 'ChangeUnderwriting', name: 'Make corrections to the underwriting.' },
                { value: 'UpgradeUnderwriting', name: 'Re-ask underwriting because of upgrade.' },
                { value: 'ContinueInterview', name: 'Ask more questions (continue with existing interview).' },
                { value: 'ProccessReferral', name: 'Procced with referred interview (underwriters only).' }
            ]
        },
        POLICY_PROGRESS: {
            STATES: [
                {name: 'Not Set', value: 'Unknown'},
                {name: 'In Progress Pre UW', value: 'InProgressPreUw'},
                {name: 'In Progress UW Referral', value: 'InProgressUwReferral'},
                {name: 'In Progress Recommendation', value: 'InProgressRecommendation'},
                {name: 'In Progress Can\'t contact', value: 'InProgressCantContact'},
                {name: 'Closed Sale', value: 'ClosedSale'},
                {name: 'Closed No Sale', value: 'ClosedNoSale'},
                {name: 'Closed Triage', value: 'ClosedTriage'},
                {name: 'Closed Can\'t Contact', value: 'ClosedCantContact'}
            ],
            FILTER: {
                IN_PROGRESS: [
                    {name: 'In Progress Pre UW', code: 'InProgressPreUw', selected: true},
                    {name: 'In Progress UW Referral', code: 'InProgressUwReferral', selected: true},
                    {name: 'In Progress Recommendation', code: 'InProgressRecommendation', selected: true},
                    {name: 'In Progress Can\'t contact', code: 'InProgressCantContact', selected: true}
                ],
                CLOSED: [
                    {name: 'Closed Sale', code: 'ClosedSale', selected: false},
                    {name: 'Closed No Sale', code: 'ClosedNoSale', selected: false},
                    {name: 'Closed Triage', code: 'ClosedTriage', selected: false},
                    {name: 'Closed Can\'t Contact', code: 'ClosedCantContact', selected: false},
                    {name: 'Not Set', code: 'Unknown', selected: false},
                ]
            }
        },
        BRAND: {
            TAL: {
                name: 'tal',
                image: '/client/images/tal-logo.png'
            },
            QA: {
                name: 'qa',
                image: '/client/images/qantas-assure-logo.png'
            },
            YB: {
                name: 'yb',
                image: '/client/images/yb-logo.png'
            }
        }
    });

})(angular.module('salesPortalApp'));


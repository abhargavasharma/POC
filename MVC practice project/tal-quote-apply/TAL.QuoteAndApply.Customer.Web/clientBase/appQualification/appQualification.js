'use strict';

angular.module('appQualification', [
    'appCustomerPortal',
    'ui.router',
    'ngSanitize'
])
.run(function () {
})
.config(function ($stateProvider) {
    $stateProvider
        .state('question-id', {
            url: '/id/:questionId'
        })
        .state('question-index', {
            url: '/index/:questionIndex'
        });
});

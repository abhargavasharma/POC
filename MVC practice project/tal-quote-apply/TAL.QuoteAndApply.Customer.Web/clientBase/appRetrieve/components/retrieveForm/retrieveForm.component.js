(function (module) {
    'use strict';
    module.directive('talRetrieveForm', function () {
        return {
            templateUrl: '/client/appRetrieve/components/retrieveForm/retrieveForm.template.html',
            restrict: 'E',
            scope: {
                quoteReferenceNumber: '@'
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talRetrieveFormController'
        };
    });

    function talRetrieveFormController(talCustomerPortalApiService, talNavigationService, talFormModelStateService) {
        var ctrl = this;
        ctrl.vm = {
            quoteReference: ctrl.quoteReferenceNumber,
            password: ''
        };

        ctrl.submit = function() {
            ctrl.loadingPromise = talCustomerPortalApiService.retrieveQuote(ctrl.vm)
                .then(function(response) {
                    talFormModelStateService.updateCleanModelState();
                    talNavigationService.handleServerRedirectAction(response.data);
                }).catch(function(response){
                    talFormModelStateService.updateModelState(response.data);
                    ctrl.retrievalError = response.data.retrieval;
                });
        };

        ctrl.inputChange = function(){
            ctrl.retrievalError = null;
        };

    }

    module.controller('talRetrieveFormController', talRetrieveFormController);
    talRetrieveFormController.$inject = ['talCustomerPortalApiService', 'talNavigationService', 'talFormModelStateService'];

})(angular.module('appRetrieve'));

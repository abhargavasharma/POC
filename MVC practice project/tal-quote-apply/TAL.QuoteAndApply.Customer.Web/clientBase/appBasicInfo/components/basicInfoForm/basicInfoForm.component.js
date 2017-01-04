(function (module) {
    'use strict';
    module.directive('talBasicInfoForm', function () {
        return {
            templateUrl: '/client/appBasicInfo/components/basicInfoForm/basicInfoForm.template.html',
            restrict: 'E',
            scope: {},
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talBasicInfoFormController'
        };
    });

    function talBasicInfoFormController($log, $scope, ngDialog, talCustomerPortalApiService, talFormModelStateService,
                                        talNavigationService, viewBagConfig, talContentService, $window, talAnalyticsService, talClientStorageService) {
        var ctrl = this;

        ctrl.showCaptcha = viewBagConfig.captchaEnabled;
        ctrl.links = talContentService.getContentByReferenceKey('shared.links');
        ctrl.isCoverCalculatorEnabled = viewBagConfig.isCalculatorEnabled;
        ctrl.isQuoteSaveLoadEnabled = viewBagConfig.isQuoteSaveLoadEnabled;

        this.genderOptions = [
            {label: 'Male', value: 'M'},
            {label: 'Female', value: 'F'}
        ];

        this.smokerOptions = [
            {label: 'Yes', value: true},
            {label: 'No', value: false}
        ];

        this.loadingPromise = talCustomerPortalApiService.initQuote().then(function (response) {
            ctrl.basicInfo = response.data;
        });

        this.submit = function () {
            talClientStorageService.setGoneViaHelpMeChoose(false);
            ctrl.basicInfo.source = 'CustomerPortalBuildMyOwn';
            ctrl.recaptchaError = '';
            ctrl.loadingPromise = talCustomerPortalApiService.createQuote(ctrl.basicInfo).then(function (response) {
                talFormModelStateService.updateCleanModelState();
                talAnalyticsService.createQuote.trackNewQuote(ctrl.basicInfo.dateOfBirth, ctrl.basicInfo.gender, ctrl.basicInfo.isSmoker,
                    ctrl.basicInfo.occupationTitle, ctrl.basicInfo.industryTitle.$$unwrapTrustedValue(), ctrl.basicInfo.annualIncome);
                ctrl.loadingPromise = talNavigationService.handleServerRedirectAction(response.data);
            }).catch(function (response) {
                talFormModelStateService.updateModelState(response.data);
                if (response.data.recaptchaResponse && response.data.recaptchaResponse.length > 0) {
                    ctrl.recaptchaError = response.data.recaptchaResponse[0];
                    if ($window.grecaptcha && $window.grecaptcha.reset) {
                        $window.grecaptcha.reset();
                    }
                }
                
            });
        };


        this.useCalcResults = function (calcResults) {
            $log.debug('use calc results');
    
            talCustomerPortalApiService.setCalcResultsForRisk(calcResults, true);
        };


    }

    module.controller('talBasicInfoFormController', talBasicInfoFormController);
    talBasicInfoFormController.$inject = ['$log', '$scope', 'ngDialog', 'talCustomerPortalApiService', 'talFormModelStateService',
        'talNavigationService', 'viewBagConfig', 'talContentService', '$window', 'talAnalyticsService', 'talClientStorageService'];

})(angular.module('appBasicInfo'));

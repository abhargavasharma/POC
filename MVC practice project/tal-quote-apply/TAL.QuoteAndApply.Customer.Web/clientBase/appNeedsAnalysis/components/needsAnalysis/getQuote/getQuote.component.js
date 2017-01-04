(function (module) {
    'use strict';
    module.directive('talGetQuote', function () {
        return {
            templateUrl     : '/client/appNeedsAnalysis/components/needsAnalysis/getQuote/getQuote.template.html',
            restrict        : 'E',
            scope           : {
                navigation: '=',
                onCompleted : '&'
            },
            bindToController: true,
            controllerAs    : 'ctrl',
            controller      : 'talGetQuoteController'
        };
    });

    function controller($log, $timeout, $stateParams,
                        talCustomerPortalApiService, talFormModelStateService, $window, viewBagConfig) {
        $log.debug('talGetQuoteController');

        var ctrl = this;

        ctrl.showCaptcha = viewBagConfig.captchaEnabled;

        ctrl.currentIndex = 0;

        if($stateParams.index){
            ctrl.currentIndex  = $stateParams.index;
        }

        ctrl.basicInfo = {};

        ctrl.genderOptions = [
            {label: 'Male', value: 'M'},
            {label: 'Female', value: 'F'}
        ];

        ctrl.smokerOptions = [
            {label: 'Yes', value: true},
            {label: 'No', value: false}
        ];

        ctrl.hasQuotePrev = function () {
            return ctrl.currentIndex > 0;
        };

        ctrl.navigation.onPrevious = function () {
            if(ctrl.currentIndex<=0){
                return;
            }

            ctrl.movingBackward = true;
            $timeout(function () {
                ctrl.currentIndex--;
                ctrl.updateNavigation();
            });
            $window.scrollTo(0, 0);
        };

        ctrl.onQuoteNext = function () {
            if(ctrl.currentIndex +1 > 3){
                if(ctrl.onCompleted){
                    ctrl.onCompleted({data: ctrl.basicInfo});
                }

                return;
            }

            ctrl.movingBackward = false;
            $timeout(function () {
                ctrl.currentIndex++;

                ctrl.updateNavigation();
            });
            $window.scrollTo(0, 0);
        };

        ctrl.updateNavigation= function () {
            ctrl.navigation.hasPrevious = ctrl.currentIndex > 0;
        };

        ctrl.isValidSmokingAnswer = function () {
            return ctrl.basicInfo.isSmoker === true || ctrl.basicInfo.isSmoker === false;
        };

        ctrl.isBirthDateQuestionValid = function () {
            if(!ctrl.basicInfo.dateOfBirth || !ctrl.basicInfo.gender || !ctrl.basicInfo.postcode ||
                ( ctrl.showCaptcha && !ctrl.basicInfo.recaptchaResponse)){
                return false;
            }

            var m = moment(ctrl.basicInfo.dateOfBirth, 'DD/MM/YYYY');
            var isValid = m.isValid();
            $log.debug('date valid: ' + isValid + ' value: ' + ctrl.basicInfo.dateOfBirth);

            return isValid;
        };

        ctrl.isOccupationValid = function () {
            return ctrl.basicInfo.annualIncome &&
                ctrl.basicInfo.occupationCode &&
                ctrl.basicInfo.industryCode;
        };

        ctrl.validateGeneralInformation = function () {

            ctrl.loadingPromise = talCustomerPortalApiService.validateGeneralInformation(ctrl.basicInfo)
                .then(function (response) {
                    $log.debug('response: ' + response);

                    //reset captcha values for next time
                    ctrl.recaptchaError = undefined;
                    ctrl.basicInfo.recaptchaResponse = undefined;

                    ctrl.onQuoteNext();
                })
                .catch(function (response) {
                    talFormModelStateService.updateModelState(response.data);
                    if (response.data.recaptchaResponse && response.data.recaptchaResponse.length > 0) {
                        ctrl.recaptchaError = response.data.recaptchaResponse[0];
                        if ($window.grecaptcha && $window.grecaptcha.reset) {
                            $window.grecaptcha.reset();
                        }
                    }
                });
        };

        ctrl.validateIncome = function () {
            
            ctrl.loadingPromise = talCustomerPortalApiService.validateIncome(ctrl.basicInfo)
                .then(function (response) {
                    $log.debug('response: ' + response);
                    ctrl.onQuoteNext();
                })
                .catch(function (response) {
                    talFormModelStateService.updateModelState(response.data);
                });
        };

    }

    module.controller('talGetQuoteController', controller);
    controller.$inject = ['$log', '$timeout', '$stateParams', 'talCustomerPortalApiService', 'talFormModelStateService', '$window', 'viewBagConfig'];

}(angular.module('appNeedsAnalysis')));
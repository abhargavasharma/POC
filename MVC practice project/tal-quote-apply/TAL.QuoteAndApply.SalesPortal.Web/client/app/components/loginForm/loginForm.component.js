'use strict';
angular.module('salesPortalApp').directive('talLoginForm', function () {
    return {
        templateUrl: '/client/app/components/loginForm/loginForm.template.html',
        restrict: 'E',
        scope: true,
        controllerAs: 'ctrl',
        bindToController: {
            sessionTimeout: '='
        },
        controller: 'talLoginFormController'
    };
});

angular.module('salesPortalApp').controller('talLoginFormController',
    function($scope, $window, talSalesPortalApiService, talFormModelStateService, FORM) {
        var ctrl = this;
        ctrl.vm = {};
        ctrl.brandSelectionVisible = false;
        ctrl.redirectLocation = '';
        ctrl.showNoBrandsAvailError = false;
        ctrl.talLogo = FORM.BRAND.TAL.image;

        function getBrandNames() {
            ctrl.brandNames = [];
            ctrl.loadingPromise = talSalesPortalApiService.getBrandNamesForUser().then(function (response) {
                _.each(response.data.brands, function(brand){
                    ctrl.brandNames.push({ name: brand, image: FORM.BRAND[brand.toUpperCase()].image});
                });
                if(ctrl.brandNames.length === 0){
                    ctrl.showNoBrandsAvailError = true;
                }
                else {
                    if (ctrl.brandNames.length === 1 || response.data.isUnderwriter) {
                        saveSelectedBrandAndRedirect(ctrl.brandNames[0].name);
                    } else {
                        ctrl.brandSelectionVisible = true;
                    }
                }
            }).catch(function () {

            });
        }

        function submitUserLogin() {
            ctrl.errorMessage = null;
            ctrl.vm.useWindowsAuth = false;
            ctrl.loadingPromise = talSalesPortalApiService.loginRequest(ctrl.vm).then(function(response){
                ctrl.redirectLocation = response.data.redirectTo;
                getBrandNames();
            }).catch(function(response){
                var modelState = response.data;
                ctrl.errorMessage = modelState.loginRequest;
                talFormModelStateService.updateModelState(modelState, $scope);
            });
        }

        function submitWindowsAuthLogin() {
            ctrl.errorMessage = null;
            ctrl.vm.useWindowsAuth = true;
            ctrl.loadingPromise = talSalesPortalApiService.loginRequest(ctrl.vm).then(function(response){
                ctrl.redirectLocation = response.data.redirectTo;
                getBrandNames();
            }).catch(function(response){

                var modelState = response.data;
                ctrl.errorMessage = modelState.loginRequest;
                talFormModelStateService.updateModelState(modelState, $scope);
            });
        }

        function saveSelectedBrandAndRedirect(brand) {
            var returnObj = { brand: brand};
            ctrl.loadingPromise = talSalesPortalApiService.saveSelectedBrand(returnObj).then(function(){
                $window.location = ctrl.redirectLocation;
            }).catch(function(){
                    //Todo how to handle this if it errors from the server
            });
        }

        ctrl.submitUserLogin = submitUserLogin;
        ctrl.submitWindowsAuthLogin = submitWindowsAuthLogin;
        ctrl.saveSelectedBrandAndRedirect = saveSelectedBrandAndRedirect;
    }
);


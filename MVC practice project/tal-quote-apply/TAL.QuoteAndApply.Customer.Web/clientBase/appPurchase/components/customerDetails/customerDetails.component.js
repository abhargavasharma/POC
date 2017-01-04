(function(module){
'use strict';
  module.directive('talCustomerDetails', function () {
      return {
        templateUrl: '/client/appPurchase/components/customerDetails/customerDetails.template.html',
        restrict: 'E',
        scope: {
            riskId: '=',
            personalDetails: '='
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talCustomerDetailsController'
      };
  });

  function talCustomerDetailsController(FORM, EVENT, $rootScope) {

      var ctrl = this;
      ctrl.originComponent = 'purchasePage';

      this.titles = FORM.PERSONAL_DETAILS.TITLES;
      this.states = FORM.ADDRESS.STATES;

      $rootScope.$on(EVENT.PURCHASE.CONTACT_DETAILS, function ($event, data) {
          //If statement prevents infinite loop
          if(data.originComponent !== ctrl.originComponent) {
              ctrl.personalDetails.firstName = data.firstName;
              ctrl.personalDetails.lastName = data.lastName;
              ctrl.personalDetails.mobileNumber = data.phoneNumber && data.phoneNumber.length > 1 && data.phoneNumber.substring(0,2) === '04' ? data.phoneNumber : ctrl.personalDetails.mobileNumber;
              ctrl.personalDetails.homeNumber = data.phoneNumber && data.phoneNumber.length > 1 && ['02', '03', '07', '08'].indexOf(data.phoneNumber.substring(0,2)) > -1 ? data.phoneNumber : ctrl.personalDetails.homeNumber;
              ctrl.personalDetails.phoneNumber = data.phoneNumber;
              ctrl.personalDetails.emailAddress = data.emailAddress;
          }
      });

      ctrl.contactDetailsChange = function () {
          $rootScope.$broadcast(EVENT.PURCHASE.CONTACT_DETAILS, {
              firstName: ctrl.personalDetails.firstName,
              lastName: ctrl.personalDetails.lastName,
              mobileNumber: ctrl.personalDetails.mobileNumber,
              homeNumber: ctrl.personalDetails.homeNumber,
              //Mobile number takes precedence in determining phoneNumber - this is needed because we still need to track phoneNumber for the clickToChat cachedContactDetails
              phoneNumber: (ctrl.personalDetails.mobileNumber === null || ctrl.personalDetails.mobileNumber === undefined) ? ctrl.personalDetails.homeNumber : ctrl.personalDetails.mobileNumber,
              emailAddress: ctrl.personalDetails.emailAddress,
              originComponent: ctrl.originComponent
          });
      };
  }

  module.controller('talCustomerDetailsController', talCustomerDetailsController );
  talCustomerDetailsController.$inject = ['FORM', 'EVENT', '$rootScope'];

})(angular.module('appPurchase'));

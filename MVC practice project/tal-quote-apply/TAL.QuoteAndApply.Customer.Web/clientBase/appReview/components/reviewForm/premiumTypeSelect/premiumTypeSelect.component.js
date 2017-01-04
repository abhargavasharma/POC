(function(module){
'use strict';
  module.directive('talPremiumTypeSelect', function () {
      return {
        templateUrl: '/client/appReview/components/reviewForm/premiumTypeSelect/premiumTypeSelect.template.html',
        restrict: 'E',
        scope: {
            premiumTotals:'=',
            selectedPremiumType:'=',
            onChangePremiumType:'=',
            paymentFrequencyPer:'='
        },
        bindToController: true,
        controllerAs: 'ctrl',
        controller: 'talPremiumTypeSelectController'
      };
    });

  function talPremiumTypeSelectController($scope, ngDialog, $window) {
      var ctrl = this;

      var showSelectableOptions;

      ctrl.getOptionClassName = function() {
          if (!ctrl.premiumTotals) {
              return;
          }

          //Calculate premium type option class and set whether to show options in 'learn more' modal
          //Using one-way binding so this function will only run by angular until it returns a result
          var numAvailableOptions = _.filter(ctrl.premiumTotals, {isEnabled: true}).length;
          showSelectableOptions = numAvailableOptions > 1; //TODO: revisit if ever adding a 3rd premium type option
          var className = 'num-options-' + numAvailableOptions;
          return className;
      };

      ctrl.showVariableSelectionDialog = function () {

          var dialog = ngDialog.open({
              templateUrl: '/client/appReview/components/reviewForm/premiumTypeSelect/modal/premiumTypeSelectionModal.template.html',
              controllerAs: 'ctrl',
              scope: $scope,
              data: {
                  selectedPremiumType: ctrl.selectedPremiumType,
                  onChangePremiumType: ctrl.onChangePremiumType,
                  paymentFrequencyPer: ctrl.paymentFrequencyPer,
                  premiumTotals: ctrl.premiumTotals,
                  showSelectableOptions: showSelectableOptions
              }
          });
          $window.scrollTo(0,0);

          dialog.closePromise.then(function (data) {

              if (data.value && data.value.premiumType) {
                  ctrl.selectedPremiumType = data.value.premiumType;
                  ctrl.onChangePremiumType(data.value);
              }
          });
      };
  }

  module.controller('talPremiumTypeSelectController', talPremiumTypeSelectController );
  talPremiumTypeSelectController.$inject = ['$scope', 'ngDialog', '$window'];

})(angular.module('appReview'));

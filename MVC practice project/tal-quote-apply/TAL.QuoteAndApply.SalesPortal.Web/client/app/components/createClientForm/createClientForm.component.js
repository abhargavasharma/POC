'use strict';
angular.module('salesPortalApp').directive('talCreateClientForm', function () {
    return {
      templateUrl: '/client/app/components/createClientForm/createClientForm.template.html',
      restrict: 'E',
      scope: true,
      controllerAs: 'ctrl',
      bindToController: {
          cancelAction:'&',
          hidePersonalDetailsSection: '=',
          isModal: '='
      },
      controller: 'talCreateClientFormController'
    };
  });

angular.module('salesPortalApp').controller('talCreateClientFormController',
  function(talSalesPortalApiService, talFormModelStateService, $scope, $window) {

    var ctrl = this;

    ctrl.loadingPromise = talSalesPortalApiService.initClient().then(function(response){
        ctrl.model = response.data;
        ctrl.model.policyOwner.ratingFactors.dateOfBirth = $scope.defaultClientData.dateOfBirth;
        ctrl.model.policyOwner.personalDetails.firstName = $scope.defaultClientData.firstName;
        ctrl.model.policyOwner.personalDetails.surname = $scope.defaultClientData.surname;
        ctrl.model.policyOwner.personalDetails.mobileNumber = $scope.defaultClientData.mobileNumber;
        ctrl.model.policyOwner.personalDetails.homeNumber = $scope.defaultClientData.homeNumber;
        ctrl.model.policyOwner.personalDetails.emailAddress = $scope.defaultClientData.emailAddress;
        ctrl.model.policyOwner.personalDetails.leadId = $scope.defaultClientData.leadId;
        ctrl.model.policyOwner.ratingFactors.gender = $scope.defaultClientData.gender;
        ctrl.model.policyOwner.personalDetails.externalCustomerReference = $scope.defaultClientData.externalCustomerReference;
    });

    ctrl.submit = function() {

        ctrl.model.policyOwner.clientType = null;        
        
      ctrl.loadingPromise = talSalesPortalApiService.createClient(ctrl.model)
          .then(function(response){
            $window.location = response.data.redirectTo;
          })
          .catch(function(response){
              var modelState = response.data;
              talFormModelStateService.updateModelState(modelState, $scope);
          });
    };

  }
);


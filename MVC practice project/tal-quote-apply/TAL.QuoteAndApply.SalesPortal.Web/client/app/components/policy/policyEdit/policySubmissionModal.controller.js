(function(module) {
    'use strict';

    module.controller('policySubmissionModalController', [
        '$scope', '$uibModalInstance','validationErrors', function($scope, $uibModalInstance, validationErrors) {
            $scope.validationErrors = validationErrors;
          
            $scope.validateClientName = function (firstname, surname) {
                return (firstname || surname);
            };

            var friendlySectionNames = {
                PersonalDetails: 'Customer Details',
                RatingFactors: 'Rating Factors',
                Underwriting: 'Underwriting',
                Quote: 'Cover Selection',
                Beneficiaries: 'Beneficiaries',
                Payment: 'Payment + Billing',
                PaymentType: 'Incorrect Payment section completed for Ownership'
            };
            
            $scope.friendlySectionName = function (sectionName) {
               
                return friendlySectionNames[sectionName];
            };

            $scope.ok = function() {
                $uibModalInstance.close();
            };
            $scope.cancel = function() {
                $uibModalInstance.dismiss();
            };
        }]);
})(angular.module('salesPortalApp'));

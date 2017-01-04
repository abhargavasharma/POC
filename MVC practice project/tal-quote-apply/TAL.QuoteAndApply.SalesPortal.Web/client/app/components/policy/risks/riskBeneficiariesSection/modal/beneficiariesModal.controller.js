
(function(module) {
    'use strict';
    
    module.controller('beneficiariesModalController', [
        '$scope', '$uibModalInstance', function($scope, $uibModalInstance) {
            $scope.ok = function() {
                $uibModalInstance.close();
            };
            $scope.cancel = function() {
                $uibModalInstance.dismiss();
            };
        }]);  
})(angular.module('salesPortalApp'));


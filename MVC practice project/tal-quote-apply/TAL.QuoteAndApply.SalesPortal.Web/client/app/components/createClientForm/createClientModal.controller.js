
(function(module) {
    'use strict';

    module.controller('createClientModalController', [
        '$scope', '$uibModalInstance', 'defaultClientData', function ($scope, $uibModalInstance, defaultClientData) {
            $scope.defaultClientData = defaultClientData;
            $scope.ok = function() {
                $uibModalInstance.close();
            };
            $scope.cancel = function() {
                $uibModalInstance.dismiss();
            };
        }]);
})(angular.module('salesPortalApp'));


'use strict';
/*
 This service will apply model state validation from the server to all registered ngModels
 The serverValidate directive can be used to register ngModels
 */
angular.module('appCustomerPortal').service('talFormModelStateService', function () {

    var srv = this;

    var ngModels = [];

    this.registerNgModel = function (key, ngModel) {
        var preExistingKey = _.find(ngModels, {key: key});
        if (preExistingKey) {
            preExistingKey.ngModel = ngModel;
        } else {
            ngModels.push({
                key: key,
                ngModel: ngModel
            });
        }
    };

    this.getModelStateForKey = function(key) {
        var ngModelObj = _.find(ngModels, {key: key});
        if (ngModelObj) {
            return ngModelObj.ngModel;
        }
        return null;
    };

    this.updateCleanModelState = function() {
        srv.updateModelState({});
    };

    this.updateModelState = function(modelState) {
        if (!modelState) {
            return;
        }
        _.each(ngModels, function(modelKeyPair) {
            if (modelState[modelKeyPair.key]) {
                modelKeyPair.ngModel.$setValidity('server', false);
                modelKeyPair.ngModel.$error.server = modelState[modelKeyPair.key][0]; //At the moment only displaying first error
            } else {
                modelKeyPair.ngModel.$setValidity('server', true);
            }

            modelKeyPair.ngModel.$setPristine();
        });

    };
});
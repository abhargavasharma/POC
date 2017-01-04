'use strict';
/*
    This service will apply model state validation from the server to all registered ngModels
    The serverValidate directive can be used to register ngModels
*/
angular.module('salesPortalApp').service('talFormModelStateService', function () {

    var srv = this;

    var ngModels = [];
    var warningKeyObjects = [];

    function broadcastValidationUiUpdate(scope) {
        scope.$broadcast('show-errors-check-validity');
    }

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

    this.triggerUiUpdate = function(scope) {
        broadcastValidationUiUpdate(scope);
    };

    this.getModelStateForKey = function(key) {
        var ngModelObj = _.find(ngModels, {key: key});
        if (ngModelObj) {
            return ngModelObj.ngModel;
        }
        return null;
    };

    var updateWarningState = function(warnings){
        _.each(warningKeyObjects, function(warningKey){
            var serverWarning = _.find(warnings, { location: warningKey.key});
            if(serverWarning){
                warningKey.message = serverWarning.message;
            }else {
                warningKey.message = null;
            }
        });
    };

    this.updateWarningState = function(warnings){
        updateWarningState(warnings);
    };

    this.updateCleanModelState = function(scope) {
        srv.updateModelState({}, scope);
    };

    this.setCleanModelStateAndApplyWarnings = function(scope, warnings) {
        srv.updateCleanModelState(scope);
        _.each(ngModels, function(modelKeyPair){
            if (warnings[modelKeyPair.key]) {
                modelKeyPair.ngModel.talWarnings = warnings[modelKeyPair.key];
            } else {
                modelKeyPair.ngModel.talWarnings = null;
            }
        });
    };

    this.updateModelState = function(modelState, scope) {

        _.each(ngModels, function(modelKeyPair){
            if (modelState[modelKeyPair.key]) {
                modelKeyPair.ngModel.$setValidity('server', false);
                modelKeyPair.ngModel.$error.server = modelState[modelKeyPair.key][0]; //At the moment only displaying first error
            } else {
                modelKeyPair.ngModel.$setValidity('server', true);
            }

            modelKeyPair.ngModel.$setPristine();
        });

        if (scope) {
            //Since we're using plugin that updates the UI based on notification, broadcast that here
            broadcastValidationUiUpdate(scope);
        }

        updateWarningState(modelState.warnings);
    };

    this.registerWarningKey = function(warningKeyObj){
        warningKeyObjects.push(warningKeyObj);
    };



});
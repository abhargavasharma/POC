(function (module) {
    'use strict';
    module.directive('talAppSave', function () {
        return {
            templateUrl: '/client/appCustomerPortal/components/appSave/appSave.template.html',
            restrict: 'E',
            scope: {
                quoteReferenceNumber: '=',
                savedStatus:'='
            },
            replace: true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talAppSaveController'
        };
    });

    function talAppSaveController($scope, $log, EVENT, ngDialog, talNavigationService, talCustomerPortalApiService,
                                  talFormModelStateService, talContentService, talAnalyticsService, viewBagConfig, $window, $rootScope) {
        var ctrl = this;

        ctrl.data =
        {
            firstName: '',
            lastName: '',
            emailAddress: '',
            phoneNumber: '',
            password: '',
            expressConsent: true //customer agrees by default
        };

        var onActivation = function () {
            $scope.loadingPromise = talCustomerPortalApiService.getContactDetails(ctrl.risk.id)
                .then(function (response) {
                    ctrl.data.firstName = response.data.firstName ? response.data.firstName : ctrl.data.firstName;
                    ctrl.data.lastName = response.data.lastName ? response.data.lastName : ctrl.data.lastName;
                    ctrl.data.emailAddress = response.data.emailAddress ? response.data.emailAddress : ctrl.data.emailAddress;
                    ctrl.data.phoneNumber = response.data.phoneNumber ? response.data.phoneNumber : ctrl.data.phoneNumber;
                    ctrl.data.expressConsent = true; //customer agrees by default
                    ctrl.data.password = '';
                })
                .catch(function () {

                });
        };

        var quoteNumber ={ quoteReferenceNumber: ctrl.quoteReferenceNumber };

        ctrl.links = talContentService.getContentByReferenceKey('shared.links');

        $scope.$on(EVENT.QUOTE.QUOTE_REF_NUMBER, function ($event, quoteRefNumber) {
            ctrl.quoteReferenceNumber = quoteRefNumber;
            quoteNumber.quoteReferenceNumber = quoteRefNumber;
        });

        ctrl.saveGateModalModel = {};
        var dialog = {};

        ctrl.showSaveGateModal = function (isForced) {
           return talCustomerPortalApiService.getRisks().then(function (response) {
               ctrl.risk = response.data[0]; //Assuming single risk for now

               if (ctrl.savedStatus === 'personalDetails' && viewBagConfig.isQuoteSaveLoadEnabled === true) {
                   ctrl.view = 'password';
               }
               else if (ctrl.savedStatus === 'personalDetails' && viewBagConfig.isQuoteSaveLoadEnabled !== true) {
                   dialog.close();
               } else {
                   ctrl.view = 'details';
               }
               
               ctrl.saveGateModalModel = {
                   quoteNumber: quoteNumber,
                   model: ctrl.data,
                   forced: isForced
               };

               var dialogParams = {
                   templateUrl: '/client/appCustomerPortal/components/appSave/modal/saveGate.modal.base.html',
                   controllerAs: 'ctrl',
                   scope: $scope,
                   data: ctrl.saveGateModalModel
               };

               if(isForced){
                   dialogParams = _.extend(dialogParams, {
                       showClose: false,
                       closeByDocument: false,
                       closeByEscape: false
                   });
               }

               dialog = ngDialog.open(dialogParams);
               
               talAnalyticsService.shared.trackSaveQuoteButton(quoteNumber.quoteReferenceNumber);
               $window.scrollTo(0, 0);
               onActivation();
           });
        };

        ctrl.submitPersonalDetails = function () {
            $scope.loadingPromise = talCustomerPortalApiService.appSaveGatePersonalDetails(ctrl.risk.id, ctrl.data)
                .then(function () {
                    if (viewBagConfig.isQuoteSaveLoadEnabled === true) {
                        talFormModelStateService.updateCleanModelState();
                        ctrl.view = 'password';
                        ctrl.savedStatus = 'personalDetails';
                        talAnalyticsService.shared.trackSaveQuotePersonalDetailsSubmit(quoteNumber.quoteReferenceNumber);
                        $window.scrollTo(0, 0);
                    } else {
                        ctrl.submitWithoutPassword();
                    }
                })
                .catch(function(response) {
                    talFormModelStateService.updateModelState(response.data);
                    throw response;
                });

            return $scope.loadingPromise;
        };

        ctrl.submitPassword = function() {
            $scope.loadingPromise = talCustomerPortalApiService.appSaveGatePassword(ctrl.risk.id, ctrl.data.password)
                .then(function(response){
                    $scope.personalDetails = response.data;
                    talFormModelStateService.updateCleanModelState();

                    ctrl.view = 'success';
                    ctrl.savedStatus = 'passwordCreated';
                    ctrl.loginCreated = true;
                    viewBagConfig.setSaveStatusAsPasswordCreated(); //Set this locally so can be in-sync with current state
                    talAnalyticsService.shared.trackSaveQuotePasswordSubmit(quoteNumber.quoteReferenceNumber);
                    dialog.closePromise.then(function() {
                        ctrl.callBackIfNeeded();
                    });
                    $window.scrollTo(0, 0);
                })
                .catch(function(response){
                    talFormModelStateService.updateModelState(response.data);

                    if(response.data.createLoginRequestError) {
                        ctrl.saveGateModalModel.errorWithSave = true;
                    }

                    throw response;
                });

            $rootScope.$broadcast(EVENT.SAVE.HIDE_MOBILE_FOOTER, {});
        };

        ctrl.submitWithoutPassword = function () {
            dialog.close();
            ctrl.callBackIfNeeded();
            $window.scrollTo(0, 0);
        };

        ctrl.callBackIfNeeded = function() {
            if (ctrl.callBackOnSuccess) {
                ctrl.callBackOnSuccess();
            }
        };

        $scope.$on(EVENT.SAVE.TRIGGER_FORCED_SAVE, function ($event, eventData) {
            $log.debug('appSave - save:trigger-forced-save. callBack: ' + JSON.stringify(eventData));
            if (eventData) {
                ctrl.callBackOnSuccess = eventData.callBack;
                ctrl.hideOnCallBack = eventData.hideOnCallBack;
            }
            if (ctrl.hasSaved) {
                ctrl.callBackIfNeeded();
                return;
            }

            ctrl.showSaveGateModal(true);
        });
    }

    module.controller('talAppSaveController', talAppSaveController);
    talAppSaveController.$inject = ['$scope', '$log', 'EVENT', 'ngDialog', 'talNavigationService',
        'talCustomerPortalApiService', 'talFormModelStateService', 'talContentService', 'talAnalyticsService',
        'viewBagConfig', '$window', '$rootScope'];
})(angular.module('appCustomerPortal'));

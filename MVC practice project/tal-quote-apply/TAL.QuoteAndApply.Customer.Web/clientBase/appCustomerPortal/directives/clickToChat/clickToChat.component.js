(function (module) {
    'use strict';
    module.directive('talClickToChat', function () {
        return {
            restrict: 'E',
            scope: {
                hide: '='
            },
            templateUrl: '/client/appCustomerPortal/directives/clickToChat/clickToChat.template.html',
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talClickToChatController'
        };
    });

    function controller($log, $scope, $rootScope, $timeout, ngDialog, EVENT, talCustomerPortalApiService, talFormModelStateService, viewBagConfig) {
        $log.debug('talClickToChatController');

        var ctrl = this;

        ctrl.originComponent = 'clickToChat';

        ctrl.defaultTab = 0;

        var chatPosition = 0;
        var callMeBackPosition = 0;

        var ensureChatPosition = function() {
            if (!chatPosition) {
                chatPosition = callMeBackPosition ? 2 : 1;
            }
        };

        var ensureCallMeBackPosition = function() {
            if (!callMeBackPosition) {
                callMeBackPosition = chatPosition ? 2 : 1;
            }
        };

        ctrl.isOpen = false;
        ctrl.requestCallbackData = {};
        ctrl.chatAvailability = {};
        ctrl.callMeBackSubmitted = viewBagConfig.callBackSubmitted;

        var onActivation = function () {
            if (ctrl.requestCallbackData.riskId) {

                ensureCallMeBackPosition();

                $scope.loadingPromise = talCustomerPortalApiService.getContactDetails(ctrl.requestCallbackData.riskId)
                    .then(function (response) {
                        //If a change hasn't come from purchase page contact details display saved details otherwise show updated inline details from purchase page
                        if(!ctrl.inlineContactDetailsChange) {
                            ctrl.requestCallbackData = response.data;
                            ctrl.broadcastContactDetails();
                        }
                        else{
                            ctrl.bindCachedContactDetails(ctrl.cachedContactDetails);
                        }
                    })
                    .catch(function () {

                    });
            }
        };

        this.onOpen = function () {
            ctrl.isOpen = true;
        };

        this.onClose = function () {
            ctrl.isOpen = false;
        };

        this.openModal = function () {
            ngDialog.closeAll();
            ngDialog.open({
                templateUrl: '/client/appCustomerPortal/directives/clickToChat/clickToChat.modal.template.html',
                scope: $scope
            });
            ctrl.isOpen = false;
            onActivation();
        };

        this.onPhone = function () {
            ensureCallMeBackPosition();
            ctrl.defaultTab = callMeBackPosition;
            ctrl.openModal();
        };

        this.onMessage = function () {
            ctrl.defaultTab = 0;
            ctrl.openModal();
        };

        this.onChat = function () {
            ensureChatPosition();
            ctrl.defaultTab = chatPosition;
            ctrl.openModal();
        };

        this.onDismissDialog = function () {
            ngDialog.closeAll();
        };

        this.onRequestCallback = function () {
            ctrl.loadingPromise = talCustomerPortalApiService.requestCallback(ctrl.requestCallbackData)
                .then(function() {
                    talFormModelStateService.updateCleanModelState();
                    ctrl.onDismissDialog();
                    ctrl.showSuccessCallbackRequest();
                    ctrl.broadcastContactDetails();
                    ctrl.cachedContactDetails = ctrl.requestCallbackData;
                    ctrl.callMeBackSubmitted = true;
                })
                .catch(function(response) {
                    talFormModelStateService.updateModelState(response.data);
                });

            return $scope.loadingPromise;
        };

        this.showSuccessCallbackRequest = function () {
            ngDialog.open({
                templateUrl: '/client/appCustomerPortal/directives/clickToChat/clickToChat.callback-success.template.html',
                scope: $scope
            });
        };

        this.broadcastContactDetails = function (){
            $rootScope.$broadcast(EVENT.PURCHASE.CONTACT_DETAILS, {
                firstName: ctrl.requestCallbackData.firstName,
                lastName: ctrl.requestCallbackData.lastName,
                mobileNumber: ctrl.requestCallbackData.mobileNumber,
                phoneNumber: ctrl.requestCallbackData.phoneNumber,
                homeNumber: ctrl.requestCallbackData.homeNumber,
                emailAddress: ctrl.requestCallbackData.emailAddress,
                originComponent: ctrl.originComponent
            });
        };

        this.onOpenChat = function () {
            if(ctrl.winref && !ctrl.winref.closed){
                ctrl.winref.focus();
                return;
            }

            ctrl.winref = window.open('', '_blank', '', true);
            ctrl.winref.document.write('Loading chat...');

            ctrl.checkChatAvailable()
                .then(function () {
                    var available = ctrl.chatAvailability.webChatIsAvailable;
                    var url = ctrl.chatAvailability.webChatUrl;

                    if(available){
                        ctrl.winref.location = url;
                    }
                    else{
                        ctrl.winref.close();
                    }
                });
        };

        this.checkChatAvailableAndCreateInteraction = function () {
            var promise = talCustomerPortalApiService.getChatAvailabilityAndCreateInteraction()
                .then(function (response) {
                    ctrl.chatAvailability = response.data;

                    if (response.data.webChatIsAvailable) {
                        ensureChatPosition();
                    }

                    $timeout(function () {
                        $rootScope.$broadcast(EVENT.CHAT.CHAT_AVAILABILITY, ctrl.chatAvailability);
                    });

                    return ctrl.chatAvailability;
                });

            ctrl.loadingPromise = promise;

            return promise;
        };

        this.checkChatAvailable = function () {
            var promise = talCustomerPortalApiService.getChatAvailability()
                .then(function (response) {
                    ctrl.chatAvailability = response.data;

                    if (response.data.webChatIsAvailable) {
                        ensureChatPosition();
                    }

                    $timeout(function () {
                        $rootScope.$broadcast(EVENT.CHAT.CHAT_AVAILABILITY, ctrl.chatAvailability);
                    });

                    return ctrl.chatAvailability;
                });

            ctrl.loadingPromise = promise;

            return promise;
        };

        $scope.$on(EVENT.RISK_ID.UPDATE, function ($event, riskId) {
            $log.debug('clickToChat - riskId: ' + riskId);
            ctrl.requestCallbackData.riskId = riskId;
        });
        
        $scope.$on(EVENT.CHAT.CALLBACK, function () {
            $log.debug('clickToChat - callback');
            ctrl.onPhone();
        });

        $scope.$on(EVENT.CHAT.CHAT_TO_AGENT, function () {
            $log.debug('clickToChat - chat to agent');
            ctrl.onOpenChat();
            ctrl.checkChatAvailableAndCreateInteraction();
        });

        $scope.$on(EVENT.CHAT.CHECK_AVAILABILITY, function () {
            $log.debug('clickToChat - check availability');
            ctrl.checkChatAvailable();
        });

        $scope.$on(EVENT.PURCHASE.CONTACT_DETAILS, function ($event, data) {
            if(data.originComponent !== ctrl.originComponent) {
                ctrl.inlineContactDetailsChange = true;
                ctrl.cachedContactDetails = data;
                ctrl.bindCachedContactDetails(data);
            }
        });

        ctrl.bindCachedContactDetails = function(data) {
            ctrl.requestCallbackData.firstName = data.firstName;
            ctrl.requestCallbackData.lastName = data.lastName;
            ctrl.requestCallbackData.mobileNumber = data.phoneNumber && data.phoneNumber.length > 1 && data.phoneNumber.substring(0,2) === '04' ? data.phoneNumber : '';
            ctrl.requestCallbackData.homeNumber = data.phoneNumber && data.phoneNumber.length > 1 && ['02', '03', '07', '08'].indexOf(data.phoneNumber.substring(0,2)) > -1 ? data.phoneNumber : '';
            //Mobile takes precedence in determining phone number
            ctrl.requestCallbackData.phoneNumber =
                ctrl.requestCallbackData.mobileNumber === null ||
                ctrl.requestCallbackData.mobileNumber === undefined ||
                ctrl.requestCallbackData.mobileNumber === '' ? ctrl.requestCallbackData.homeNumber : ctrl.requestCallbackData.mobileNumber;
            ctrl.requestCallbackData.emailAddress = data.emailAddress;
        };

        ctrl.checkChatAvailable();
    }

    module.controller('talClickToChatController', controller);
    controller.$inject = ['$log', '$scope', '$rootScope', '$timeout', 'ngDialog', 'EVENT', 'talCustomerPortalApiService', 'talFormModelStateService', 'viewBagConfig'];

}(angular.module('appCustomerPortal')));


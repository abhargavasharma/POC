(function (module) {
    'use strict';
    module.directive('talChatToAgentLink', function () {
        return {
            restrict: 'E',
            scope: {
                chatAvailability: '=',
                prefix: '@',
                suffix: '@'
            },
            transclude: true,
            templateUrl: '/client/appCustomerPortal/directives/chatToAgentLink/chatToAgentLink.template.html',
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talChatToAgentLinkController'
        };
    });

    function controller($log, $scope, $rootScope, EVENT) {
        $log.debug('talChatToAgentLinkController');

        var ctrl = this;

        ctrl.onChat = function () {
            //triggers an event handled in clickToChat.component to display popup for request a callback
            $rootScope.$broadcast(EVENT.CHAT.CHAT_TO_AGENT);
        };


        $scope.$on(EVENT.CHAT.CHAT_AVAILABILITY, function ($event, chatAvailability) {
            $log.debug('chat-to-agent-link - chatAvailability: ' + JSON.stringify(chatAvailability));
            ctrl.chatAvailability = chatAvailability;
        });
    }

    module.controller('talChatToAgentLinkController', controller);
    controller.$inject = ['$log', '$scope', '$rootScope', 'EVENT'];

}(angular.module('appCustomerPortal')));


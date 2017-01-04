'use strict';

angular.module('appCustomerPortal').service('talNavigationService', ['$window', '$q', '$timeout', 'pageSpinnerService', function ($window, $q, $timeout, pageSpinnerService) {

    this.handleServerRedirectAction = function(redirectAction) {
        if (redirectAction.redirectTo) {

            pageSpinnerService.start();

            //Keep the user in suspense and give them time to marvel at the full page spinner before transitioning pages
            $timeout(function() {
                $window.location = redirectAction.redirectTo;
            }, 2000);

            //Return non-resolving promise so we can show loading in the UI while switching pages
            return $q.defer().promise;
        }

        throw 'No redirect action';
    };
}]);
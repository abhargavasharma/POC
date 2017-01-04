(function(module){
    'use strict';
    module.directive('talTalusUi', function () {
        return {
            templateUrl: '/client/app/components/talusUi/talusUi.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk:'=',
                quoteReferenceNumber: '=',
                onActivation: '=',
                syncInterviewCallBack: '='
            },
            controller: 'talTalusUiController',
            link: function(scope) {
                scope.reloadUnderwritingFrame = function(url){
                    var iframeHtml = '<iframe id="talusUiIframe" src="' + url + '"></iframe>';

                    var frameWrapper = angular.element(document.querySelector('#frameWrapper'))[0];
                    frameWrapper.innerHTML = iframeHtml;
                };
            }
        };
    });

    function talTalusUiController($scope, $sce, $window, talSalesPortalApiService, NOTIFICATIONS, $q) {

        var eventMethod = $window.addEventListener ? 'addEventListener' : 'attachEvent';
        var eventer = $window[eventMethod];

        var messageEvent = eventMethod === 'attachEvent' ? 'onmessage' : 'message';

        var interviewLoadedPromises = [];
        var deferInterviewLoaded = null;

        var createAndRegisterInterviewLoadedPromise = function() {
            //Need to keep track of all deferInterviewLoaded promises we create (due to phoenix window event not always firing if user navigates away from the underwriting tab while phoenix is still loading)
            //We did try resolving/nulling the existing promise before creating a new one but that still didn't work.
            //These promises will all be resolved on the last successful phoenix load (see resolveAllInterviewLoadedPromises())
            deferInterviewLoaded = $q.defer();
            interviewLoadedPromises.push(deferInterviewLoaded);
        };

        var resolveAllInterviewLoadedPromises = function() {
            //Resolve all deferInterviewLoaded (to include orphan ones that may not have resolved)
            while(interviewLoadedPromises.length) {
                var promise = interviewLoadedPromises.pop();
                promise.resolve();
                promise = null;
            }
        };

        var syncInterviewAndReload = function(){
            if ($scope.syncInterviewCallBack) {
                $scope.syncInterviewCallBack().then(function () {
                    onActivation();
                });
            }
        };

        eventer(messageEvent, function (e) {
            if(e.data) {
                if(e.data.event === NOTIFICATIONS.UNDERWRITING.QUESTION_ANSWERED) {
                    // Doing nothing now, because we expect a sync to happen 
                    // when we leave the underwriting section
                }
                if(e.data.event === NOTIFICATIONS.UNDERWRITING.INTERVIEW_LOADED) {
                    resolveAllInterviewLoadedPromises();
                }
                if(e.data.event === NOTIFICATIONS.UNDERWRITING.INTERVIEW_PRECONDITION_ERROR) {
                    syncInterviewAndReload(onActivation);
                }
                if(e.data.event === NOTIFICATIONS.UNDERWRITING.OUTCOME_OVERRIDEN ||
                    e.data.event === NOTIFICATIONS.UNDERWRITING.LOADINGS_OVERRIDEN ||
                    e.data.event === NOTIFICATIONS.UNDERWRITING.EXCLUSIONS_OVERRIDEN) {
                    // Doing nothing now, because we expect a sync to happen 
                    // when we leave the underwriting section
                }
                if(e.data.event === NOTIFICATIONS.UNDERWRITING.NOTE_ADDED) {
                    // Doing nothing now, because we expect a sync to happen 
                    // when we leave the underwriting section
                }
            }
        }, false);

        var onActivation = function() {
            var promises = [];

            createAndRegisterInterviewLoadedPromise();

            promises.push(deferInterviewLoaded.promise);

            $scope.loadingPromise = $q.all(promises);

            talSalesPortalApiService.getRiskTalusUiUrl($scope.quoteReferenceNumber, $scope.risk.riskId).then(function(response){
                $scope.risk.talusUiUrl = response.data;
                $scope.reloadUnderwritingFrame($sce.trustAsResourceUrl($scope.risk.talusUiUrl));
            });

        };

        $scope.onActivation = onActivation;
    }

    module.controller('talTalusUiController', talTalusUiController );
    talTalusUiController.$inject = ['$scope', '$sce', '$window', 'talSalesPortalApiService', 'NOTIFICATIONS', '$q'];

})(angular.module('salesPortalApp'));

(function(module) {
        'use strict';
        module.directive('talPolicyNotesTexts',
            function() {
                return {
                    templateUrl: '/client/app/components/policy/policyNotesSection/policyNotesSection.template.html',
                    restrict: 'E',
                    scope: {
                        risk: '=',
                        section:'=',
                        quoteReferenceNumber: '=',
                        readOnly: '='
                    },
                    controller: 'talNoteController'
                };
            });


        function talNoteController($scope, talSalesPortalApiService, talPolicySectionsNotificationService) {
            $scope.notesResults = {};
            var onActivation = function () {
                $scope.section.isCompleted = true;
                $scope.section.isModelValid = true;
                $scope.loadingPromise = talSalesPortalApiService.getNotes($scope.quoteReferenceNumber)
                .then(function(response) {
                    $scope.notesResults = response.data;
                })
                .catch(function() {
                    $scope.notesResults = getEmptyNotesResults();
                });

            function getEmptyNotesResults() {
                return {
                    notes: []
                    };
                }
            };

            $scope.section.onActivationEvent = onActivation;
            
            talPolicySectionsNotificationService.registerNoteAddedEvent(function () {
               onActivation();
            });
        }

    module.controller('talNoteController', talNoteController);
    talNoteController.$inject = ['$scope', 'talSalesPortalApiService', 'talPolicySectionsNotificationService' ];

})(angular.module('salesPortalApp'));
(function(module){
'use strict';
  module.directive('talPolicyNotes', function () {
      return {
        templateUrl: '/client/app/components/policy/policyNotes/policyNotes.template.html',
        restrict: 'E',
        scope: {
            quoteNumber:'='
        },
        controller: 'talPolicyNotesController'
      };
    });

  function talPolicyNotesController($scope, talSalesPortalApiService, talWindowCloseService, talFormModelStateService, talPolicySectionsNotificationService) {
      $scope.expanded = false;
      $scope.sessionPolicyNote = {
          id: null,
          noteText: ''
      };

      $scope.toggleExpanded = function() {
          $scope.expanded = !$scope.expanded;
      };

      $scope.updateNote = function() {
          //Only update note to server if we have something to send
          if ($scope.sessionPolicyNote.id || $scope.sessionPolicyNote.noteText) {
              $scope.updatingPromise = talSalesPortalApiService.updatePolicyNote($scope.quoteNumber, $scope.sessionPolicyNote)
                  .then(function (response) {
                      $scope.sessionPolicyNote.id = response.data.noteId;
                      talFormModelStateService.updateCleanModelState($scope);
                      talPolicySectionsNotificationService.onNoteAddedChange();
                  })
                  .catch(function(response){
                      talFormModelStateService.updateModelState(response.data, $scope);
                  });
          }
      };

      talWindowCloseService.registerOnCloseEvent(function() {
          $scope.updateNote();
      });
  }

  module.controller('talPolicyNotesController', talPolicyNotesController );
  talPolicyNotesController.$inject = ['$scope', 'talSalesPortalApiService', 'talWindowCloseService', 'talFormModelStateService', 'talPolicySectionsNotificationService'];

})(angular.module('salesPortalApp'));


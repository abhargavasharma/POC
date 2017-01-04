(function(module){
'use strict';
  module.directive('talUnderwriterFilter', function () {
      return {
        templateUrl: '/client/app/components/dashboard/underwriterDashboard/underwriterFilter/underwriterFilter.template.html',
        restrict: 'E',
        scope: {},
        controller: 'talUnderwriterFilterController'
      };
    });

  function talUnderwriterFilterController($scope, $window, talSalesPortalApiService, FORM) {

      $scope.openStartModel = {};
      $scope.openEndModel = {};
      $scope.openStartDate = function($event, elementOpened) {
          $event.preventDefault();
          $event.stopPropagation();

          $scope.openStartModel[elementOpened] = !$scope.openStartModel[elementOpened];
      };

      $scope.openEndDate = function($event, elementOpened) {
          $event.preventDefault();
          $event.stopPropagation();

          $scope.openEndModel[elementOpened] = !$scope.openEndModel[elementOpened];
      };

      //------------Everything below here is for calendar functionality----------

      $scope.submitModel = {
          startDate: '',
          endDate: '',
          InProgress: true,
          Unresolved: true,
          Resolved: false,
          assignedTo: '',
          DTH: true,
          TPS: true,
          TRS: true,
          IP: true
      };

      Date.prototype.addDays = function(days)
      {
          var dat = new Date(this.valueOf());
          dat.setDate(dat.getDate() + days);
          return dat;
      };

      $scope.initStartDate = function() {
          var today = new Date().addDays(-30);
          $scope.submitModel.startDate = today;
      };
      $scope.initStartDate();
      $scope.initEndDate = function() {
          var today = new Date();
          $scope.submitModel.endDate = today;
      };
      $scope.initEndDate();

      //-------------Everything else below here is for the referrals and filtering---------------

      $scope.filterModel = {
          planTypes: FORM.REFERRAL.FILTER.PLAN_TYPES,
          referralStatuses: FORM.REFERRAL.FILTER.STATUSES,
          assignedTo: []
      };

      $scope.lookupReferralState = function(state){
          var status = _.find(FORM.REFERRAL.STATES, function(status){
              return status.value === state;
          });
          return status.name;
      };

      $scope.searchResults = [];

      var onActivation = function() {
          $scope.loadingPromise = talSalesPortalApiService.getUnderwriterReferrals().then(function(response){
              $scope.searchResults = response.data.referrals;
              _.each($scope.searchResults, function(result){
                  result.disabled = result.state === 'Resolved';
              });
              $scope.filterModel.assignedTo = response.data.underwriters;
          });
      };

      $scope.filterOpen = true;

      $scope.toggleFilter = function(){
          $scope.filterOpen = !$scope.filterOpen;
      };

      $scope.searchPerformed = true;

      $scope.filteredReferrals = {};

      $scope.hasReferralResults = function(){
          return $scope.filteredReferrals && $scope.filteredReferrals.length > 0;
      };

      $scope.goToQuote = function(quoteId){
          $window.location = '/Policy/Edit/' + quoteId;
      };

      $scope.searchResults = onActivation();

      $scope.updateAssignedTo = function(quoteReferenceNumber, user){
          $scope.loadingPromise = talSalesPortalApiService.updateAssignedTo(quoteReferenceNumber, { name: user}).then(function(response){
              var referralToUpdate = _.find($scope.searchResults, function(referral){
                return referral.quoteReferenceNumber === response.data.quoteReferenceNumber;
              });
              referralToUpdate.state = response.data.state;
          });
      };
  }

  module.controller('talUnderwriterFilterController', talUnderwriterFilterController );
  talUnderwriterFilterController.$inject = ['$scope', '$window', 'talSalesPortalApiService', 'FORM'];

})(angular.module('salesPortalApp'));


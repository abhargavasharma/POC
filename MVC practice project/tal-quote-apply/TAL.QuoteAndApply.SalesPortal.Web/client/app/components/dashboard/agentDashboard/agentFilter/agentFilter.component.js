(function(module){
'use strict';
  module.directive('talAgentFilter', function () {
      return {
        templateUrl: '/client/app/components/dashboard/agentDashboard/agentFilter/agentFilter.template.html',
        restrict: 'E',
        scope: {
            externalRefRequired: '=',
            externalRefLabel: '='
        },
        controller: 'talAgentFilterController'
      };
    });

  function talAgentFilterController($scope, $window, talSalesPortalApiService, FORM) {

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

      $scope.submitModel = {
          startDate: '',
          endDate: '',
          InProgressPreUw: true,
          InProgressUwReferral: true,
          InProgressRecommendation: true,
          InProgressCantContact: true,
          ClosedSale: false,
          ClosedNoSale: false,
          ClosedTriage: false,
          ClosedCantContact: false,
          Unknown: false,
          pageNumber: 1
      };

      //------------Everything below here is for calendar functionality----------

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
          inProgressTypes: FORM.POLICY_PROGRESS.FILTER.IN_PROGRESS,
          closedTypes: FORM.POLICY_PROGRESS.FILTER.CLOSED
      };

      $scope.searchResults = [];

      var onActivation = function() {
          $scope.loadingPromise = talSalesPortalApiService.getAgentQuotes($scope.submitModel).then(function(response){
              $scope.searchResults = response.data.quotes;

              $scope.paging = {
                  totalRecords: response.data.totalRecords,
                  currentPage: response.data.currentPage,
                  pageCount: response.data.pageCount
              };
          });
      };

      $scope.updateAgentQuotes = function(){
          $scope.submitModel.pageNumber = 1;
          onActivation();
      };

      $scope.filterOpen = true;

      $scope.toggleFilter = function(){
          $scope.filterOpen = !$scope.filterOpen;
      };

      $scope.getProgress = function(progress){
          var progressOption = _.find(FORM.POLICY_PROGRESS.STATES, function(prog){
              return prog.value === progress;
          });
          if(progressOption) {
              return progressOption.name;
          }
          return 'Not Set';
      };

      $scope.searchPerformed = true;

      $scope.filteredQuotes = {};

      $scope.goToQuote = function(quoteId){
          $window.location = '/Policy/Edit/' + quoteId;
      };

      $scope.hasResults = function() {
        return $scope.paging.totalRecords > 0 && $scope.searchResults.length > 0;
      };

      $scope.onPreviousPage = function(){
          $scope.submitModel.pageNumber = $scope.submitModel.pageNumber -1;
          onActivation();
      };

      $scope.onNextPage = function(){
          $scope.submitModel.pageNumber = $scope.submitModel.pageNumber +1;
          onActivation();
      };

      $scope.searchResults = [];
      $scope.paging = {
          totalRecords: 0,
          currentPage: 0,
          pageCount: 0
      };
      onActivation();
  }

  module.controller('talAgentFilterController', talAgentFilterController );
    talAgentFilterController.$inject = ['$scope', '$window', 'talSalesPortalApiService', 'FORM'];

})(angular.module('salesPortalApp'));


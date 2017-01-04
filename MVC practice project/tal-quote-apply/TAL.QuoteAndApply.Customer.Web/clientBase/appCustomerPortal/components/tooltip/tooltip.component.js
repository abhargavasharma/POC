(function(module){
  'use strict';

  var uniqueId = 0;

  module.directive('talTooltip', function () {
    return {
      templateUrl: '/client/appCustomerPortal/components/tooltip/tooltip.template.html',
      restrict: 'E',
      transclude:true,
      scope: {
        'label': '@'
      },
      controller: 'tooltipController'
    };
  });

  var tooltipController = function($rootScope, $scope, $document, $element, talBrowserService) {
    $scope.isDisplayed = false;
    $scope.isTouch = talBrowserService.isTouch();

    $scope.id = uniqueId;
    uniqueId += 1;

    // -------- Event callbacks ------
    
    var closeTooltip = function() {
      $scope.isDisplayed = false;
      $scope.$digest();
    };

    var openTooltip = function() {
      $scope.isDisplayed = true;
      $scope.$digest();
    };

    var toggleTooltip = function() {
      if ($scope.isDisplayed) {
        closeTooltip();
      } else {
        openTooltip();
      }
    };

    // Should close the tooltip is pressed.
    angular.element($document).on('keyup', function(e){
      if(e.keyCode === 27){
        $scope.isDisplayed = false;
        $scope.$digest();
      }
    });

   
    // The trigger
    if($scope.isTouch){
      $element
        .bind('touchstart', function(event) {
          toggleTooltip();

          //to stop the parent element (i.e. button) click/touch event
          event.preventDefault();
        });
    } else {
      $element
        .bind('mouseenter', function() {
          openTooltip();
        })
        .bind('mouseleave', function() {
          closeTooltip();
        });
    }
   
      

    if(!$scope.isTouch){
      $element.children().eq(0)
        .bind('focusin', function() {
          openTooltip();
        })
        .bind('focusout', function() {
          closeTooltip();
        })
        .bind('keypress', function(e) {
          // On "enter" or "space"
          if (e.keyCode === 32 || e.keyCode === 13) {
            toggleTooltip();
          }
      });
    }
  };

 

  module.controller('tooltipController', tooltipController);
  tooltipController.$inject = ['$rootScope','$scope', '$document', '$element', 'talBrowserService'];
})(angular.module('appCustomerPortal'));

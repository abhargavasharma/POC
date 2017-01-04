(function (module) {
  'use strict';

  var SELECTORS = {
    TAB: '.tal-tabs__nav-item, .tal-tabs__ec-head-header, .tal-tabs__pane-header'
  };

  var tabTabsDirective = function($timeout, bpChangeEvent) {
    var uniqueId = 1;

    return {
      restrict: 'E',
      transclude: true,
      scope: {
        tabsAt: '@',
        allowMultiOpen: '@',
        defaultTab: '@?',
        tabsApi: '=',
        isHTabs: '@',
        onChange: '&'
      },
      controller: function($scope, $element) {
        var panes = $scope.panes = [];
        $scope.tabsId = 'tabs-' + uniqueId;
        uniqueId += 1;

        $scope.isHTabs = $scope.isHTabs || false;

        // Setup tabs
        
        // At and above the tabsAt attribute breakpoint, behavior is tabs.
        // Below it, it's expand-collapse.
        // -----------------------------------------------------------
        
        $scope.isTabs = false;
        $scope.focusedIndex = false;

        /**
         * Set the focus to tab with the given index.
         */
        var setFocusToTabByIndex = function(element, index) {
          if (index === false) { return; }
          $timeout(function() {
            element[0].querySelectorAll(SELECTORS.TAB)[index].focus();
          }, 40);
        };

        /**
         * Returns the index of the currently focused tab. If none are focused, returns false;
         *
         * @method     findFocusedTab
         * @param      {<type>}  element  { description }
         * @return     {<type>}  { description_of_the_return_value }
         */
        var findFocusedTab = function(element) {
          var currentlyFocusedTab = false;

          angular.forEach(element[0].querySelectorAll(SELECTORS.TAB), function(el, index) {
            if (el.isEqualNode(document.activeElement)) {
              currentlyFocusedTab = index;
            }
          });

          return currentlyFocusedTab;
        };

        /**
         * Handles switching the behavior to the component from expand/collapse to tabs,
         * based on the breakpoint.
         */
        if ($scope.tabsAt) {
          bpChangeEvent.on(function(bp) {

            // On large bp, be tabs
            if (bp.isIn('m')) {
              $timeout(function() {
                $scope.isTabs = true;
                angular.forEach(panes, function(pane) {
                  pane.isTabs = true;
                  pane.selected = false;
                });

                if ($scope.focusedIndex !== false) {
                  panes[$scope.focusedIndex].selected = true;
                } else {
                  panes[$scope.defaultTab].selected = true;
                }

                setFocusToTabByIndex($element, $scope.focusedIndex);
              });
            }

            // Else be accordion
            else {
              $timeout(function() {
                $scope.isTabs = false;
                angular.forEach(panes, function(pane) {
                  pane.isTabs = false;
                });

                setFocusToTabByIndex($element, $scope.focusedIndex);
              });
            }
          });
        }


        /**
         * Select the next non-disabled tab.
         */
        var selectNext = function selectNext() {
          var targetPaneIndex = 0; // default

          // Find the next tab
          angular.forEach(panes, function(pane, index) {
            if (pane.selected) {
              targetPaneIndex = (index + 1) % panes.length;
            }
          });

          // Skip any disabled tabs
          while (panes[targetPaneIndex].enabled === false) {
            targetPaneIndex = (targetPaneIndex + 1) % panes.length;
          }

          // Deselect all tabs
          if (!$scope.allowMultiOpen || $scope.isTabs) {
            angular.forEach(panes, function(pane) {
              pane.selected = false;
            });
          }

          $timeout(function() {
            $element[0].querySelectorAll(SELECTORS.TAB)[targetPaneIndex].focus();
            panes[targetPaneIndex].selected = true;
            $scope.focusedIndex = findFocusedTab($element);
            panes[targetPaneIndex].$emit('tab-selected', targetPaneIndex);
          });
        };


        /**
         * Select the previous non-disabled tab.
         */
        var selectPrev = function selectPrev() {
          var targetPaneIndex = 0; // default

          // Find the previous tab
          angular.forEach(panes, function(pane, index) {
            if (pane.selected) {
              targetPaneIndex = (index - 1 + panes.length) % panes.length;
            }
          });

          // Skip any disabled tabs
          while (panes[targetPaneIndex].enabled === false) {
            targetPaneIndex = (targetPaneIndex - 1) % panes.length;
          }

          // Deselect all tabs
          if (!$scope.allowMultiOpen || $scope.isTabs) {
            angular.forEach(panes, function(pane) {
              pane.selected = false;
            });
          }

          $timeout(function() {
            $element[0].querySelectorAll(SELECTORS.TAB)[targetPaneIndex].focus();
            panes[targetPaneIndex].selected = true;
            $scope.focusedIndex = findFocusedTab($element);
            panes[targetPaneIndex].$emit('tab-selected', targetPaneIndex);
          });
        };

        /**
         * Resize the current tab to fit it's content.
         */
        var resize = function resize() {
          // Find the previous tab
          angular.forEach(panes, function(pane, index) {
            if (pane.selected) {
              panes[index].$emit('tab-selected', index);
            }
          });
        };

        $scope.resize = resize;


        /**
         * Handler for the keydown event
         *
         * @param      {object}  event   The event.
         */
        var onKeydown = function onKeydown(event) {
          var isInvalidTarget = true;

          // Is the event target an appropriate nav element for the tabs or ec?
          _.forEach(event.target.classList, function(className) {
            if (SELECTORS.TAB.indexOf(className) !== -1 ) {
              isInvalidTarget = false;
            }
          });

          // If target is invalid, then don't navigate... it's probably a form element within the content panel.
          if (isInvalidTarget) {
            return;
          }

          // If target is valid, then handle the event
          if (event.keyCode === 39 || event.keyCode === 40) {
            selectNext();
            event.preventDefault();
          } else if (event.keyCode === 37 || event.keyCode === 38) {
            selectPrev();
            event.preventDefault();
          }
        };



        $element
          .on('keydown', onKeydown);



        $scope.select = function(pane) {
          var currentState = pane.selected;

          if (($scope.isTabs && pane.selected) || !pane.enabled) {
            return;
          }

          if (!$scope.allowMultiOpen || $scope.isTabs) {
            angular.forEach(panes, function(pane) {
              pane.selected = false;
            });
          }

          pane.selected = !currentState;

          $scope.focusedIndex = findFocusedTab($element);
          pane.$emit('tab-selected', $scope.focusedIndex);

          if (pane.onChange) {
              pane.onChange();
          }
        };

        this.select = $scope.select;

        var addedIndex = 0;

        this.addPane = function(pane) {
          panes.push(pane);
          pane.tabsId = $scope.tabsId;
          
          if (addedIndex === Number($scope.defaultTab)) {
            pane.selected = true;
          }
          addedIndex = addedIndex + 1;
        };

        $scope.tabsApi = {
          next: selectNext,
          prev: selectPrev,
          resize: resize,
        };
      },
      templateUrl: '/client/appCustomerPortal/components/expandCollapse/ecTabs.template.html'
    };
  };

  var talTabPaneDirecive = function() {
    var uniqueId = 1;

    return {
      require: '^^talTabs',
      restrict: 'E',
      transclude: true,
      scope: {
          title: '@',
          talId: '=',
          enabled: '=',
          onChange: '&'
      },
      link: function(scope, element, attrs, tabsCtrl) {
        scope.tabId = 'tab-' + uniqueId;
        uniqueId += 1;

        scope.enabled = (scope.enabled !== undefined) ? scope.enabled : true;

        tabsCtrl.addPane(scope);

        scope.select = function() {
            tabsCtrl.select(scope);
            if (scope.onChange) {
                scope.onChange();
            }
        };
      },
      templateUrl: '/client/appCustomerPortal/components/expandCollapse/ecPane.template.html'
    };
  };

  tabTabsDirective.$inject = ['$timeout', 'bpChangeEvent'];

  module
    .directive('talTabs', tabTabsDirective)
    .directive('talTabPane', talTabPaneDirecive);

})(angular.module('appCustomerPortal'));
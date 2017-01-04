'use strict';

angular.module('styleguide')
  .controller('IntroCtrl', ['$scope', '$timeout', function ($scope, $timeout) {
      $scope.data = {
        index : 0
      };

      $scope.items1 = [
        {
          title: 'TOTAL & PERMANENT DISABILITY (TPD) COVER',
          isOn: true,
          value: 200000,
          premium: 123,
          classCols: 'col-lg-3'
        },
        {
          title: 'LIFE COVER',
          isOn: true,
          value: 200000,
          premium: 123,
          classCols: 'col-lg-3',
        },
        {
          title: 'CRITICAL ILLNESS (CI) COVER',
          isOn: true,
          value: 200000,
          premium: 123,
          classCols: 'col-lg-3',
        },
        {
          title: 'INCOME PROTECTION COVER',
          isOn: true,
          value: 200000,
          premium: 123,
          classCols: 'col-lg-3',
        }
      ];


      // ---- Into JS configuration ------------------------------------------------------------

  		$scope.introOptions = {
  				steps:[
  					{
  						element: '.tal-card tal-form-switch',
  						intro: '<h3 class="tour-heading h5">Switch cover on/off</h3> <p class="p--small">You can switch these four cover types on or off depending on your needs.</p>',
  						position: 'top'
  					},
  					{
  						element: '.tal-card form-select-textfield',
  						intro: '<h3 class="tour-heading h5">Change cover amount</h3> <p class="p--small">You can select from several cover amounts or enter an amount of your choosing.  If you\'re unsure how much cover you need you can select the \'help\' option to generate an estimate.</p>',
  						position: 'top'
  					},
  					{
  						element: '.tal-card .tal-btn--secondary',
  						intro: '<h3 class="tour-heading h5">Options</h3> <p class="p--small">You can customise your policy by including or excluding certain conditions. Use the tooltips to find out more about each one.</p>',
  						position: 'top'
  					},
  					{
  						element: '.tal-card .tal-form-checkbox-list',
  						intro: '<h3 class="tour-heading h5">Attach to life</h3> <p class="p--small">For a reduction on your premium you can make TPD and/or Critical Illness Cover part of your Life policy.</p>',
  						position: 'top'
  					}
  				],
  				showStepNumbers: false,
  				exitOnOverlayClick: true,
  				exitOnEsc: true,
  				nextLabel: 'Got it!',
  				skipLabel: 'Skip Tutorial',
  				doneLabel: 'Done'
  			};

  		$scope.shouldAutoStart = false;

  		$scope.onBeforeChangeEvent = function () {
  			angular.element(document.querySelector('.is-clone')).remove();
  		};

  		$scope.onAfterChangeEvent = function (targetElement) {
  			var elementClone = targetElement.cloneNode(true),
  				highlightRegion = document.querySelector('.introjs-tooltipReferenceLayer'),
  				appended;

  			angular.element(document.querySelector('.is-clone')).remove();

  			angular.element(elementClone).addClass('is-faded').addClass('is-clone');
  			appended = highlightRegion.appendChild(elementClone);

        $scope.tourIndex = $scope.tourIndex || 0;

  			$timeout(function() {
  				angular.element(appended).removeClass('is-faded');
  				$scope.tourIndex += 1;
  			}, $scope.tourIndex === 0 ? 0 : 550);
  		};
  }]);
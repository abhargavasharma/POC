'use strict';
angular.module('salesPortalApp').service('talLoadingOverlayService', function (spinnerService) {

    this.showFullPageOverlay = function() {
        spinnerService.show('fullOverlaySpinner');
    };

    this.hideFullPageOverlay = function() {
        spinnerService.hide('fullOverlaySpinner');
    };

    this.showUnderwritingSpinner = function () {
        spinnerService.show('updateUnderwritingSpinner');
    };

    this.hideUnderwritingSpinner = function () {
        spinnerService.hide('updateUnderwritingSpinner');
    };
    
});
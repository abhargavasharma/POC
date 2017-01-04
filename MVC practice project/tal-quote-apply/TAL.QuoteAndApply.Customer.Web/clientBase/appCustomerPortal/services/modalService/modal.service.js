'use strict';
angular.module('appCustomerPortal').service('talModalService', function (ngDialog, viewBagConfig, $window) {

    this.showTimeoutModal = function() {

        var hasSaved = viewBagConfig.saveStatus === 'passwordCreated';
        var startOverUrl = viewBagConfig.journeySource === 'CustomerPortalHelpMeChoose' ? '/NeedsAnalysis' : '/';

        var dialogParams = {
            templateUrl: '/client/appCustomerPortal/services/modalService/sessionTimeout.modal.template.html',
            showClose: false,
            closeByDocument: false,
            closeByEscape: false,
            controllerAs: 'ctrl',
            data: {
                quoteNumber: viewBagConfig.quoteReference,
                hasSaved: hasSaved,
                startOverUrl: startOverUrl
            }
        };

        ngDialog.open(dialogParams);
        $window.scrollTop = 0;
    };
});
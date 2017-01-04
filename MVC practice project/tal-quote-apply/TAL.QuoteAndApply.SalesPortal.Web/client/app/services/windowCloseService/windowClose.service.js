/**
 * Created by abell on 22/01/2016.
 */
'use strict';

angular.module('salesPortalApp').service('talWindowCloseService', function ($window) {

    var closeEvents = [];

    var onWindowUnload = function() {
        _.each(closeEvents, function(closeEvent){
            closeEvent();
        });
    };

    this.registerOnCloseEvent = function(closeFunction) {
        closeEvents.push(closeFunction);
    };

    $window.onbeforeunload = onWindowUnload;
});
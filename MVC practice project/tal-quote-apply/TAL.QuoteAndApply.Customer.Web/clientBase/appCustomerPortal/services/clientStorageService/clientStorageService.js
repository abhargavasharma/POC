'use strict';

angular.module('appCustomerPortal').service('talClientStorageService', function ($cookies) {

    /* Service for storing client data locally */

    var keys = {
        shownWalkthrough: 'tal-quote-and-apply-walkthrough',
        goneViaHelpMeChoose: 'tal-quote-and-apply-help-me-choose'
    };

    function setValue(key, value) {
       $cookies.put(key, value);
    }

    function getValue(key) {
        return $cookies.get(key);
    }

    function getBoolValue(key) {
        return getValue(key) === 'true'; //Since cookies store everything as a string, return simple bool conversion here
    }

    this.setShownWalkthrough = function(value) {
        setValue(keys.shownWalkthrough, value);
    };

    this.getShownWalkthrough = function() {
        return getValue(keys.shownWalkthrough);
    };

    this.setGoneViaHelpMeChoose = function(value) {
        setValue(keys.goneViaHelpMeChoose, value);
    };

    this.getGoneViaHelpMeChoose = function() {
        return getBoolValue(keys.goneViaHelpMeChoose);
    };

});
/*
    Global setup for all specs
 */
'use strict';
beforeEach(function () {
    module(function ($provide) {
        var mockViewBagConfig = {
            debugEnabled: false
        };

        $provide.constant('viewBagConfig', mockViewBagConfig);
    });
});

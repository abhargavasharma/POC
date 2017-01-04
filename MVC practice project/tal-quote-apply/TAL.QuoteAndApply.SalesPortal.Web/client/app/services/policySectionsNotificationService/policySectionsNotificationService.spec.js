'use strict';

describe('Service: talPolicySectionsNotificationService', function () {

    // load the service's module
    beforeEach(module('salesPortalApp'));

    // instantiate service
    var talPolicySectionsNotificationService, $timeout;
    beforeEach(inject(function (_talPolicySectionsNotificationService_, _$timeout_) {
        talPolicySectionsNotificationService = _talPolicySectionsNotificationService_;
        $timeout = _$timeout_;
    }));

    it('should do something', function () {
        expect(!!talPolicySectionsNotificationService).toBe(true);
    });

    describe('activateSection', function() {
        it('should behave in some way', function () {

            expect(true).toBe(true);
        });

    });
});

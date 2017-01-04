'use strict';

describe('Filter: Dollar dropdown', function () {

    var filter;

    beforeEach(module('appCustomerPortal'));

    beforeEach(inject(function (_$filter_) {
        filter = _$filter_('dollarsDropdown');
    }));

    it('should exist', inject(function() {
        expect(filter).not.toBeNull();
    }));

    it('should return the custom label if the custom value is used', function() {
        expect(filter('custom')).toBe('custom');
    });

    it('should return a formated dollar number if ana integer is provided', function() {
        expect(filter(1000)).toBe('1,000');
    });
});
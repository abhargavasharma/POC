'use strict';

describe('Filter: Period', function () {

    var filter;

    beforeEach(module('appCustomerPortal'));

    beforeEach(inject(function (_$filter_) {
        filter = _$filter_('period');
    }));

    it('should exist', inject(function() {
        expect(filter).not.toBeNull();
    }));

    it('should return m if type is inline', function() {
        expect(filter('monthly', 'inline')).toBe('m');
    });

    it('should return month if type is review', function() {
        expect(filter('monthly', 'review')).toBe('month');
    });
    it('should return y if type is inline', function() {
        expect(filter('annual', 'inline')).toBe('y');
    });

    it('should return year if type is review', function() {
        expect(filter('annual', 'review')).toBe('year');
    });
});
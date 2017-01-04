'use strict';

describe('Filter: Dollar Part', function () {

    var dollarPart;

    beforeEach(module('appCustomerPortal'));

    beforeEach(inject(function (_$filter_) {
        dollarPart = _$filter_('dollarPart');
    }));

    it('should exist', inject(function() {
        expect(dollarPart).not.toBeNull();
    }));

    it('should be a number', function() {
        expect(typeof dollarPart(10.45)).toBe('number');
        expect(dollarPart(10.45)).toMatch(/d*/);
    });

    it('should round down the the nearest whole number', function() {
        expect(dollarPart(10)).toBe(10);
        expect(dollarPart(9.999999)).toBe(9);
        expect(dollarPart(0.45)).toBe(0);
        expect(dollarPart(10.454)).toBe(10);
    });
});
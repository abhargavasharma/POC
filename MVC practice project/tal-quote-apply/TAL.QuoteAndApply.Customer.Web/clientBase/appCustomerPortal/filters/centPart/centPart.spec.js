(function() {
    'use strict';

    describe('Filter: Cent Part', function () {

        var centPart;

        beforeEach(module('appCustomerPortal'));

        beforeEach(inject(function (_$filter_) {
            centPart = _$filter_('centPart');
        }));

        it('should exist', inject(function() {
            expect(centPart).not.toBeNull();
        }));

        it('should be a number', function() {
            expect(typeof centPart(10.45)).toBe('string');
            expect(centPart(10.45)).toMatch(/d*/);
        });

        it('should return 00 when given whole numbers', function() {
            expect(centPart(10)).toBe('00');
            expect(centPart(10.0)).toBe('00');
            expect(centPart(10.00)).toBe('00');
            expect(centPart(10.00000)).toBe('00');
        });

        it('should always have length 2', function() {
            expect(centPart(0).length).toBe(2);
            expect(centPart(1).length).toBe(2);
            expect(centPart(5.3).length).toBe(2);
            expect(centPart(10.00).length).toBe(2);
            expect(centPart(10.00000).length).toBe(2);
            expect(centPart(10.342234).length).toBe(2);
        });

        it('should return the correct number of cents', function() {
            expect(centPart(0)).toBe('00');
            expect(centPart(0.23)).toBe('23');
            expect(centPart(5.04)).toBe('04');
            expect(centPart(0.99)).toBe('99');
            expect(centPart(0.0999)).toBe('09');
            expect(centPart(10.30)).toBe('30');
        });
    });
}());
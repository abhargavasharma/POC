(function () {
    'use strict';

    describe('Filter: Numbers to Words', function () {

        var numberToWords;

        beforeEach(module('appCustomerPortal'));

        beforeEach(inject(function (_$filter_) {
            numberToWords = _$filter_('numberToWords');
        }));

        it('should return one when 1 is the input', function () {
            expect(numberToWords(1)).toBe('one');
        });

        it('should return two when 2 is the input', function () {
            expect(numberToWords(2)).toBe('two');
        });

        it('should return three when 3 is the input', function () {
            expect(numberToWords(3)).toBe('three');
        });

        it('should return four when 4 is the input', function () {
            expect(numberToWords(4)).toBe('four');
        });

    });
}());
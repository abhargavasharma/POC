'use strict';

describe('Component: Progress Bar', function () {
	var scope, compile;

    beforeEach(module('appCustomerPortal'));
		beforeEach(module('htmlTemplates'));

		beforeEach(inject(function(_$rootScope_, _$compile_) {
		  compile = _$compile_;
		  scope = _$rootScope_.$new();
		}));

    describe('Total', function () {
        var iScope, element;

        it('should be 100 by default', function() {
            element = '<tal-progress-bar></tal-progress-bar>';
            element = compile(element)(scope);
            scope.$digest();
            iScope = element.isolateScope();

            expect(iScope.total).toEqual(100);
        });

        it('should be possible to change inline', function() {
            element = '<tal-progress-bar total="35"></tal-progress-bar>';
            element = compile(element)(scope);
            scope.$digest();
            iScope = element.isolateScope();

            expect(Number(iScope.total)).toEqual(35);
        });
    });

    describe('Progress', function () {
        var iScope, element;

        beforeEach(inject(function() {
            element = '<tal-progress-bar></tal-progress-bar>';
            element = compile(element)(scope);
            scope.$digest();
            iScope = element.isolateScope();
        }));

        it('should default to 0', function() {
            expect(Number(iScope.progress)).toEqual(0);
        });
    });

    describe('Percentage', function () {
    	var iScope, element;

    	beforeEach(inject(function() {
    	    element = '<tal-progress-bar></tal-progress-bar>';
    	    element = compile(element)(scope);
    	    scope.$digest();
    	    iScope = element.isolateScope();
    	}));

    	it('should be a number', function() {
    		expect(typeof iScope.percentage).toEqual('number');
    	});

        it('should be equal to progress/total, as a percentage', function() {
            iScope.$apply(function() {
                iScope.progress = 10;
                iScope.total = 20;
            });
            expect(iScope.percentage).toEqual(50);

            iScope.$apply(function() {
                iScope.progress = 15;
                iScope.total = 60;
            });
            expect(iScope.percentage).toEqual(25);
        });

    	it('should track progress', function() {
    		iScope.$apply(function() {
    			iScope.progress = 50;
    		});
    		expect(iScope.percentage).toEqual(50);

    		iScope.$apply(function() {
    			iScope.progress = 10;
    		});
    		expect(iScope.percentage).toEqual(10);

    		iScope.$apply(function() {
    			iScope.progress = 100;
    		});
    		expect(iScope.percentage).toEqual(100);
    	});

    	it('should bottom out at 0', function() {
    		iScope.$apply(function() {
    			iScope.progress = -1;
    		});
    		expect(iScope.percentage).toEqual(0);
    	});

    	it('should top out at 100', function() {
    		iScope.$apply(function() {
    			iScope.progress = 101;
                iScope.total = 100;
    		});
    		expect(iScope.percentage).toEqual(100);
    	});

    	it('should be tracked by the bar element\'s width, as a percentage', function() {
    		iScope.$apply(function() {
    			iScope.progress = 0;
    		});

    		expect($(element).find('.tal-pbar__bar').css('width')).toEqual('0%');

    		iScope.$apply(function() {
    			iScope.progress = 57;
    		});
    		
    		expect($(element).find('.tal-pbar__bar').css('width')).toEqual('57%');
    	});
    });
});
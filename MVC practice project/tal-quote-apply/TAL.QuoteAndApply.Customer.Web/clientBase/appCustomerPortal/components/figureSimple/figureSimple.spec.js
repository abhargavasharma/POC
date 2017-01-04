'use strict';

describe('Component: Figure Simple:', function () {
	var scope,
		compile,
		element,
		template = '<tal-figure-simple value="figValue"></tal-figure-simple>',
		iScope;

	beforeEach(module('appCustomerPortal'));
	beforeEach(module('htmlTemplates'));

	beforeEach(inject(function(_$rootScope_, _$compile_) {
	  compile = _$compile_;
	  scope = _$rootScope_.$new();
	}));

	beforeEach(inject(function() {
		element = compile(template)(scope);
		scope.$digest();
		iScope = element.isolateScope();
	}));

	describe('Value', function() {
		it('should be a number', function() {
			expect(typeof iScope.value).toEqual('number');
		});
	});

	describe('figValue', function() {
		it('should track the value', function() {
			iScope.$apply(function() {
				iScope.value = 1000.45;
			});
			expect(iScope.figValue).toEqual(1000.45);
		});
	});

	describe('figValueOld', function() {
		it('should track the previous value', function() {
			iScope.$apply(function() {
				iScope.value = 1000.45;
			});
			iScope.$apply(function() {
				iScope.value = 30.30;
			});
			expect(iScope.figValueOld).toEqual(1000.45);
		});
	});
});
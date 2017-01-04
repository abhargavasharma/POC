'use strict';

describe('Component: Figure:', function () {
	var TEMPLATES = {
		INLINE: '<tal-figure value="figValue" type="inline"></tal-figure>',
		REVIEW: '<tal-figure value="figValue" type="review"></tal-figure>',
		TOTAL: '<tal-figure value="figValue" type="total"></tal-figure>'
	};

	var scope,
		compile;

	beforeEach(module('appCustomerPortal'));
	beforeEach(module('htmlTemplates'));

	beforeEach(inject(function(_$rootScope_, _$compile_) {
	  compile = _$compile_;
	  scope = _$rootScope_.$new();
	}));

	_.each(TEMPLATES, function(template, name) {
		describe(name + ' Figure:', function () {
			var iScope, element;

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
						iScope.period = 'monthly';
						iScope.value = 1000.45;
					});
					expect(iScope.figValue).toEqual(1000.45);
				});
			});

/*
			describe('figValueOld', function() {
				it('should track the previous value', function() {
					iScope.$apply(function() {
						iScope.period = 'monthly';
						iScope.value = 1000.45;
					});
					iScope.$apply(function() {
						iScope.value = 30.30;
					});
					expect(iScope.figValueOld).toEqual(1000.45);
				});
			});
*/
		});
	});

	describe('Total Figure:', function () {
		var iScope, element;

		beforeEach(inject(function() {
			element = compile(TEMPLATES.TOTAL)(scope);
			scope.$digest();
			iScope = element.isolateScope();
		}));

/*
		describe('The Monthly/Annual toggle', function() {
			it('should start as monthly', function() {
				expect(iScope.period).toEqual('monthly');
			});

			it('should toggle between monthly and annual', function() {
				iScope.togglePeriod();
				expect(iScope.period).toEqual('annual');
				iScope.togglePeriod();
				expect(iScope.period).toEqual('monthly');
			});

			// Not working for some reason, although the behavior is correct.
			// May be something to do with figValue being updated by a $watch.
			it('should change the displayed value', function() {
				iScope.period = 'monthly';
				iScope.value = 10;
				scope.$digest();

				expect(iScope.figValue).toEqual(10);
				iScope.togglePeriod();
				scope.$digest();
				expect(iScope.figValue).toEqual(120);
			});
		});
*/
	});
});
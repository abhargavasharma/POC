'use strict';

describe('Component: formSelectTextField | ', function () {
	var scope, compiler, $timeout;
	// load the service's module
	beforeEach(module('appCustomerPortal'));
	beforeEach(module('htmlTemplates'));

	beforeEach(inject(function($rootScope, $compile, _$timeout_) {
		compiler = $compile;
		$timeout = _$timeout_;
		// First element
		scope = $rootScope.$new();

		scope.data = {
			value:1000,
			selectItems:[1000, 2000],
			placeholder:'Choose a value...'
		};
	}));
		
	describe('It always ', function() {
		var element, myScope;

		beforeEach(function() {
			element = '<form-select-textfield placeholder="Choose a value..." ng-model="data.value" form-select-items="data.selectItems"></form-select-textfield>';
			element = compiler(element)(scope);
			scope.$digest();
			myScope = element.isolateScope();
		});

		it('adds a custom item to the selection', function() {
			expect(myScope.ctrl.formItems['0']).toBe('custom');
		});

		it('does not add the extra custom item to the original selection', function() {
			expect(scope.data.selectItems['0']).not.toBe('custom');
		});

		it('sets the selection to the correct initial value', function() {
			expect(myScope.ctrl.selection).toBe(scope.data.value);
		});

		it('sets the selection to an empty string if custom is selected', function() {
			myScope.ctrl.selection = 'custom';
			scope.$digest();
			expect(scope.data.value).toBe('');

			myScope.ctrl.selection = 2000;
			scope.$digest();
			expect(scope.data.value).toBe(2000);
		});

		it('should focus on the input when set to custom', function() {
			myScope.ctrl.selection = 'custom';

			scope.$digest();
			spyOn(element[0].querySelector('input'),'focus');

			$timeout.flush();

			expect(element[0].querySelector('input').focus).toHaveBeenCalled();
		});

		it('has an "is-empty" css class applied if empty.', function() {
			myScope.ctrl.ngModel = 10000;
			scope.$digest();
			myScope.ctrl.ngModel = null;
			scope.$digest();
			expect(element.hasClass('is-empty')).toBe(true);
		});

		it('has no "is-empty" css class applied if it has a non-zero value', function() {
			myScope.ctrl.ngModel = null;
			scope.$digest();
			myScope.ctrl.ngModel = 10000;
			scope.$digest();
			expect(element.hasClass('is-empty')).toBe(false);
		});
	});
});
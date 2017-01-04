'use strict';

describe('Component: formSelect', function () {
	var scope, compiler;
    // load the service's module
    beforeEach(module('appCustomerPortal'));
    beforeEach(module('htmlTemplates'));

    beforeEach(inject(function($rootScope, $compile) {
    	compiler = $compile;
		// First element
		scope = $rootScope.$new();

		scope.data = {
			value:'opt1',
			selectItems:['opt1', 'opt2'],
			placeholder:'my placeholder'
		};
		
	}));
    
    describe('when there is a no value by default and no placeholder', function(){
    	var element, myScope;
		beforeEach(function(){
			element = '<form-select  ng-model="dummy" form-select-items="data.selectItems"></form-select>';
			element = compiler(element)(scope);
			scope.$digest();

			myScope = element.isolateScope();
		});
		describe('has a selectedValue', function(){
	    	
		    it('is taking the placeholder value by default', function () {
		        expect(myScope.ctrl.placeholder).toBe('Please select');
		    });
	    });
	});
    describe('when there is a value', function(){
    	var element, myScope;
		beforeEach(function(){
			element = '<form-select placeholder="{{data.placeholder}}" ng-model="data.value" form-select-items="data.selectItems"></form-select>';
			element = compiler(element)(scope);
			scope.$digest();

			myScope = element.isolateScope();
		});
		describe('has a selectedValue', function(){
		    it('is taking the value of the ngModel', function () {
		        expect(myScope.ctrl.selectedValue).toBe('opt1');
		    });
		    it('updates when the model changes', function () {
		    	scope.data.value = 'opt2';
		    	scope.$digest();
		        expect(myScope.ctrl.selectedValue).toBe('opt2');
		    });
	    });
	});
});
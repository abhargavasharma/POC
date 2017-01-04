'use strict';

describe('Component: testComponent', function () {
	var element1, element2, scope, myScope1, myScope2, compiler;

    // load the service's module
    beforeEach(module('appCustomerPortal'));
    beforeEach(module('htmlTemplates'));

 
    beforeEach(inject(function($rootScope, $compile) {
    	compiler = $compile;
    	// First element

		scope = $rootScope.$new();
		scope.data = {
			value:'M'
		};

		scope.myMethod = function(){

		};
		spyOn(scope, 'myMethod');
    }));
    describe('has a setModelValue method', function(){

    	beforeEach(function(){
    		element1 = '<form-radio on-option-changed="myMethod()" class="tal-form-radio" name="testRadio" value="opt1" ng-model="data.value" tal-form-id="radio1">Option1</form-radio>';
			element1 = compiler(element1)(scope);

			element2 = '<form-radio on-option-changed="myMethod()" class="tal-form-radio" name="testRadio" value="opt2" ng-model="data.value" tal-form-id="radio2">Option2</form-radio>';
			element2 = compiler(element2)(scope);
			scope.$digest();

			myScope1 = element1.isolateScope();
			myScope2 = element2.isolateScope();
    	});
    	it('triggers an onOption method when called', inject(function($timeout) {
    	    myScope1.setModelValue();
	        $timeout.flush();
	    	expect(scope.myMethod).toHaveBeenCalled();
	    }));
    });
});
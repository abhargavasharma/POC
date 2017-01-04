'use strict';

describe('Component: formSwitch', function () {
    var element, scope, myScope;

    // load the service's module
    beforeEach(module('appCustomerPortal'));
    beforeEach(module('htmlTemplates'));

    beforeEach(inject(function($rootScope, $compile) {

        // First element
        scope = $rootScope.$new();

		scope.data = {
			myFlag:false
		};
		element = '<tal-form-switch ng-model="data.myFlag"></tal-form-switch>';
		element = $compile(element)(scope);
		scope.$digest();

        myScope = element.isolateScope();
        // 
    }));
    it('should have a isActive in the isolated scope', function () {
        expect(myScope.isActive).toBeDefined();
        expect(myScope.isActive).toBe(false);

    });
    it('should update isActive if model change', function(){
        scope.data.myFlag = true;
        scope.$digest();
        expect(myScope.isActive).toBe(true);
    });
    it('should toggle isActive toggle function is called', function(){
        expect(myScope.isActive).toBe(false);
        myScope.toggle();
        expect(myScope.isActive).toBe(true);
        myScope.toggle();
        expect(myScope.isActive).toBe(false);
    });
});


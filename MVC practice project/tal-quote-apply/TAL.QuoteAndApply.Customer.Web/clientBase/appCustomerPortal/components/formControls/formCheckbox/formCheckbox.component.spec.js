'use strict';

describe('Component: Form Checkbox:', function () {
  var scope, compiler;

  // load the service's module
  beforeEach(module('appCustomerPortal'));
  beforeEach(module('htmlTemplates'));

  beforeEach(inject(function($rootScope, $compile) {
    compiler = $compile;
    // First element

    scope = $rootScope.$new();
    scope.data = {
      value:false
    };
  }));
/*
//TODO: Kirill will fix these up when he makes the changes to component in the styleguide
  describe('when the checkbox is enabled', function(){
    var element, myScope;
    beforeEach(function(){
      element = '<form-checkbox tal-name="checkbox11" tal-id="checkbox11" class="tal-form-checkbox" ng-model="data.value">My label</form-checkbox>';
      element = compiler(element)(scope);

      scope.$digest();

      myScope = element.isolateScope();
    });
    it('should do something', function () {
      expect(myScope.ctrl.ngModel).toBe(false);
      myScope.toggleCheck();
      expect(myScope.ctrl.ngModel).toBe(true);
      myScope.toggleCheck();
      expect(myScope.ctrl.ngModel).toBe(false);
    });
  });
  
  describe('when the checkbox is disabled', function(){
    var element, myScope;
    beforeEach(function(){
      element = '<form-checkbox tal-name="checkbox11" tal-id="checkbox11" class="tal-form-checkbox" ng-model="data.value"  disabled="disabled">My label</form-checkbox>';
      element = compiler(element)(scope);

      scope.$digest();

      myScope = element.isolateScope();
    });
    it('should do something', function () {
      expect(myScope.ctrl.ngModel).toBe(false);
      myScope.toggleCheck();
      expect(myScope.ctrl.ngModel).toBe(false);
    });
  });
*/
});
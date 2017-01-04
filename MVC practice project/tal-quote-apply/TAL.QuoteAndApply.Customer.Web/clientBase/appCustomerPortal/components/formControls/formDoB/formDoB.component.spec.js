'use strict';

describe('Component: testComponent', function () {
  // var element, myScope;
  var scope, compiler;

    // load the service's module
    beforeEach(module('appCustomerPortal'));

    beforeEach(module('htmlTemplates'));

    it('should do something', function () {
        expect(true).toBe(true);
    });
    beforeEach(inject(function($rootScope, $compile) {
      compiler = $compile;
      // First element

      scope = $rootScope.$new();

      scope.data = {
        value:'22/12/2015'
      };
    }));
    describe('when the value is not provided', function(){
      var element, myScope;
      beforeEach(function(){
        element = '<form-dob ng-model="data.value2" > </form-dob>';
        element = compiler(element)(scope);

        scope.$digest();

        myScope = element.isolateScope();
      });
      describe('sets all fields to 0', function(){
        it('is defined', function () {
          expect(myScope.ctrl.day).toBe('00');
          expect(myScope.ctrl.month).toBe('00');
          expect(myScope.ctrl.year).toBe('0000');
        });
        
      });
    });
    describe('when the value is provided', function(){
      var element, myScope;
      beforeEach(function(){
        element = '<form-dob ng-model="data.value" > </form-dob>';
        element = compiler(element)(scope);

        scope.$digest();

        myScope = element.isolateScope();
      });
      describe('has a day field', function(){
        it('is defined', function () {
          expect(myScope.ctrl.day).toBeDefined();
        });
        it('initialises based on ngModel', function () {
          expect(myScope.ctrl.day).toBe('22');
        });
        it('updates the ngModel if the field is changed', function(){
          myScope.ctrl.day = 23;
          scope.$digest();
          expect(scope.data.value).toBe('23/12/2015');
        });
      });
      describe('has a month field', function(){
        it('is defined', function () {
          expect(myScope.ctrl.month).toBeDefined();
        });
        it('initialises based on ngModel', function () {
          expect(myScope.ctrl.month).toBe('12');
        });
        it('updates the ngModel if the field is changed', function(){
          myScope.ctrl.month = 11;
          scope.$digest();
          expect(scope.data.value).toBe('22/11/2015');
        });
      });
      describe('has a year field', function(){
        it('is defined', function () {
          expect(myScope.ctrl.year).toBeDefined();
        });
        it('initialises based on ngModel', function () {
          expect(myScope.ctrl.year).toBe('2015');
        });
        it('updates the ngModel if the field is changed', function(){
          myScope.ctrl.year = 2014;
          scope.$digest();
          expect(scope.data.value).toBe('22/12/2014');
        });
      });
      describe('has a year field', function(){
        it('updates the field if the model changes', function () {
          expect(scope.data.value).toBe('22/12/2015');
          expect(myScope.ctrl.day).toBe('22');
          expect(myScope.ctrl.month).toBe('12');
          expect(myScope.ctrl.year).toBe('2015');
          scope.data.value = '23/11/2014';
          scope.$digest();
          expect(myScope.ctrl.day).toBe('23');
          expect(myScope.ctrl.month).toBe('11');
          expect(myScope.ctrl.year).toBe('2014');

        });
        it('does not update if the date is invalid', function(){
          expect(scope.data.value).toBe('22/12/2015');
          scope.data.value = '1/1/14';
          scope.$digest();
          expect(myScope.ctrl.day).toBe('22');
          expect(myScope.ctrl.month).toBe('12');
          expect(myScope.ctrl.year).toBe('2015');
        });
      });
      describe('has a date validator', function(){
        it('is defined', function () {
          expect(myScope.validateDate).toBeDefined();
        });
        it('validates dates', function () {
          expect(myScope.validateDate('22/12/2014')).toBe(true);
          expect(myScope.validateDate('22-12-2014')).toBe(false);
          expect(myScope.validateDate('22-12-15')).toBe(false);
          expect(myScope.validateDate('01/02/2015')).toBe(true);
          expect(myScope.validateDate('1/2/2015')).toBe(false);
        });
      });
    });
});
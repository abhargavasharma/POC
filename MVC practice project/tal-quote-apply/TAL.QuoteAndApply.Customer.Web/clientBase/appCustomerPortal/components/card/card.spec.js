'use strict';


describe('Component: card', function () {
  var element, scope, compiler;
  // load the service's module
  beforeEach(module('appCustomerPortal'));
  beforeEach(module('htmlTemplates'));

  beforeEach(inject(function($rootScope, $compile) {
    compiler = $compile;
    // First element

    scope = $rootScope.$new();
    element = '<tal-card></tal-card>';
    element = compiler(element)(scope);
    scope.$digest();
  }));

  it('has no test yet', function(){
    expect(true).toBe(true);
  });
});
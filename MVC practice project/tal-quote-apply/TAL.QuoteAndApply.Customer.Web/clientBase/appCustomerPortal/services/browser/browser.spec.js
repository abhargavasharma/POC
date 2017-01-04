'use strict';
describe('The browser service',function(){
  var myService;

  // '$q','errorMessage','waitflag','$http'
  beforeEach(function (){
    
    // load the module.
    module('appCustomerPortal');
    
    // inject your service for testing.
    // The _underscores_ are a convenience thing
    // so you can have your variable name be the
    // same as your injected service.
    inject(function( _talBrowserService_) {
      myService = _talBrowserService_;
    });
  });

  describe('has a isTouch method that', function(){
    it('is defined', function(){
      expect(myService.isTouch).toBeDefined();
    });
  });

});
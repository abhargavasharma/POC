'use strict';
describe('Component: tooltip', function () {
	var eventStub, scope, myScope, compiler, element, rootScope, $document, triggerKeyUp, talBrowserService;
    // load the service's module
    beforeEach(module('appCustomerPortal'));
    beforeEach(module('htmlTemplates'));

    beforeEach(inject(function($rootScope, $compile, _$document_, _talBrowserService_) {
      talBrowserService = _talBrowserService_;
    	$document = _$document_;
    	compiler = $compile;
    	rootScope = $rootScope;
    	// First element
    	eventStub = {
    		preventDefault:function(){},
    		stopPropagation:function(){}

    	};

    	triggerKeyUp = function (key) {
  		  var event = document.createEvent('Event');
  		  event.keyCode = key;
  		  event.initEvent('keyup');
  		  document.dispatchEvent(event);
  	  };

      spyOn(talBrowserService, 'isTouch').andCallFake(function() {
        return false;
      });

    	spyOn(eventStub, 'preventDefault');
    	spyOn(eventStub, 'stopPropagation');

  		scope = $rootScope.$new();
  		scope.data = {
  			value:false
  		};
        element = '<tal-tooltip label="this is my label">This is the content of my tooltip</tal-tooltip>';
        element = compiler(element)(scope);
        scope.$digest();
        myScope = element.isolateScope();

    }));

    var checkOpen = function (){
        expect(myScope.isDisplayed).toBe(true);
    };
    var checkClose = function (){
        expect(myScope.isDisplayed).toBe(false);
    };
    // @TODO Add in unit tests for the events and their handling

    // THIS TEST FAILS. IGNORING FOR MASTER

    it('sets isDisplayed to true on mouseenter', function(){
      myScope.isDisplayed = false;
      element.triggerHandler({
        type : 'mouseenter'
      });
      checkOpen();
    });
    it('sets isDisplayed to false on mouseleave', function(){
      myScope.isDisplayed = true;
      element.triggerHandler({
        type : 'mouseleave'
      });
      checkClose();
    });
    it('sets isDisplayed to true on focus in', function(){
      myScope.isDisplayed = false;
      element.children().eq(0).triggerHandler({
        type : 'focusin'
      });
      checkOpen();
    });
    it('sets isDisplayed to true on focus out', function(){
      myScope.isDisplayed = true;
      element.children().eq(0).triggerHandler({
        type : 'focusout'
      });
      checkClose();
    });
    it('sets isDisplayed to true on keypress space or enter ', function(){
      myScope.isDisplayed = false;
      element.children().eq(0).triggerHandler({
        type : 'keypress',
        keyCode:32
      });
      checkOpen();
      element.children().eq(0).triggerHandler({
        type : 'keypress',
        keyCode:32
      });
      checkClose();
    });
    it('sets isDisplayed to true on keypress space or enter ', function(){
      myScope.isDisplayed = false;
      element.children().eq(0).triggerHandler({
        type : 'keypress',
        keyCode:13
      });
      checkOpen();
      element.children().eq(0).triggerHandler({
        type : 'keypress',
        keyCode:13
      });
      checkClose();
    });
    it('sets isDisplayed to false if esc is pressed', function(){
      myScope.isDisplayed = true;
      triggerKeyUp(27);
      checkClose();
    });
});

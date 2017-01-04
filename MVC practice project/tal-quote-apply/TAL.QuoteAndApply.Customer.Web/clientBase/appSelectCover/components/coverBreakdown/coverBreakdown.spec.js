'use strict';

describe('Component: cover breakdown', function () {
  var scope, compiler;
  // load the service's module
  beforeEach(module('appSelectCover'));
  beforeEach(module('htmlTemplates'));

  beforeEach(inject(function($rootScope, _$compile_) {
    compiler = _$compile_;

    scope = $rootScope.$new();

    scope.period = 'monthly';

    scope.breakdown = [
       
    ];

  }));
  describe('has a total value', function(){
    var element, myScope;
    beforeEach(function(){
        element = '<tal-cover-breakdown period="period" tal-cover-breakdown-data="breakdown"></tal-cover-breakdown>';
        
        element = compiler(element)(scope);
        scope.$digest();

        myScope = element.isolateScope();
      });
    it('is defined', function(){
      expect(myScope.total).toBeDefined();
    });
    
    it('is is equal 0 by default', function(){
      expect(myScope.total).toBe(0);
    });

    it('is adds all the breakdown from the data', function(){
      scope.breakdown = [
        {
            label:'Life Cover',
            value:21
        },
        {
            label:'Permanent Disability Cover',
            value:35
        },
        {
            label:'Critical illness Cover',
            value:10
        },
        {
            label:'Income Protection Cover',
            value:12
        }
      ];
      scope.$digest();
      expect(myScope.total).toBe(78);
    });
  });
  
});
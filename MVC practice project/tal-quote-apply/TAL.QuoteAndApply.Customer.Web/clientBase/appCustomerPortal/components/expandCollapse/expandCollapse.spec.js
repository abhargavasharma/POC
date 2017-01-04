'use strict';

describe('Component: Tabs/Expand Collapse:', function () {
	var DD,
		scope,
		compile,
		template = [
			'<tal-tabs>' +
			  '<tal-tab-pane title="1">' +
			  '</tal-tab-pane>' +
			  '<tal-tab-pane title="2">' +
			  '</tal-tab-pane>' +
			  '<tal-tab-pane title="3">' +
			  '</tal-tab-pane>' +
			  '<tal-tab-pane title="4">' +
			  '</tal-tab-pane>' +
			'</tal-tabs>'
		].join('');
    beforeEach(module('appCustomerPortal'));
	beforeEach(module('htmlTemplates'));

	beforeEach(inject(function(_$rootScope_, _$compile_, _ddProvider_) {
		DD = _ddProvider_.dd;
		compile = _$compile_;
		scope = _$rootScope_.$new();
	}));

    describe('Expand Collapse', function () {
    	var iScope, element;

    	beforeEach(inject(function() {
    	    element = compile(template)(scope);
    	    scope.$digest();
    	    iScope = element.isolateScope();
    	}));

    	// it('should be an tabs at s bp', function() {
    	// 	scope.$apply(function() {
    	// 		iScope.tabsAt = 's'
    	// 	});

    	// 	expect(iScope.isTabs).toBe(true);
    	// });

    	// it('should be tabs at s bp', function() {
    	// 	expect(iScope.isTabs).toBe(true);
    	// });
    });
});
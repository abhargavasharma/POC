'use strict';

describe('Component: Main Nav', function () {
	var scope, compile;

    beforeEach(module('appCustomerPortal'));
	beforeEach(module('htmlTemplates'));

	beforeEach(inject(function(_$rootScope_, _$compile_) {
	  compile = _$compile_;
	  scope = _$rootScope_.$new();
	}));
});
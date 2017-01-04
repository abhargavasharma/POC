'use strict';

describe('Component: basicInfoForm', function () {

    // load the service's module
    beforeEach(module('appBasicInfo'));
    beforeEach(module('htmlTemplates'));

    var basicFormController,
        $httpBackend,
        rootScope,
        viewBagConfig,
        analyticsService;

    beforeEach(inject(function ($rootScope, $q, $controller, _$httpBackend_, _talCustomerPortalApiService_, _talFormModelStateService_, _talNavigationService_) {
        rootScope = $rootScope;
        $httpBackend = _$httpBackend_;


        viewBagConfig = {};
        viewBagConfig.enableCaptcha = false;

        analyticsService = {};
        analyticsService.createQuote = {};
        analyticsService.createQuote.trackNewQuote = function () { };

        spyOn(_talCustomerPortalApiService_, 'initQuote').andCallFake(function(){
            var mockPromise = $q.defer();
            mockPromise.resolve({data: {something: 'something'}});
            return mockPromise.promise;
        });

        basicFormController = $controller('talBasicInfoFormController', {
            '$scope': $rootScope.$new(),
            'talCustomerPortalApiService' : _talCustomerPortalApiService_,
            'talFormModelStateService': _talFormModelStateService_,
            'talNavigationService': _talNavigationService_,
            'viewBagConfig': viewBagConfig,
            'talAnalyticsService': analyticsService
        });
    }));

    it('should set basicInfo to init quote object returned from service', function () {
        rootScope.$apply(); //Needed so that promise will resolve when calling initQuote
        expect(basicFormController.basicInfo).toNotBe(undefined);
        expect(basicFormController.basicInfo.something).toBe('something');
    });

    it('should set correct gender options', function () {
        expect(basicFormController.genderOptions).toNotBe(undefined);
        expect(basicFormController.genderOptions[0].label).toBe('Male');
        expect(basicFormController.genderOptions[0].value).toBe('M');
        expect(basicFormController.genderOptions[1].label).toBe('Female');
        expect(basicFormController.genderOptions[1].value).toBe('F');
    });

    it('should set correct smoker options', function () {
        expect(basicFormController.smokerOptions).toNotBe(undefined);
        expect(basicFormController.smokerOptions[0].label).toBe('Yes');
        expect(basicFormController.smokerOptions[0].value).toBe(true);
        expect(basicFormController.smokerOptions[1].label).toBe('No');
        expect(basicFormController.smokerOptions[1].value).toBe(false);
    });

});

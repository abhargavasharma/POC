
'use strict';

describe('Component: singleSelectOptionButtons', function () {

    // load the service's module
    beforeEach(module('appCustomerPortal'));
    beforeEach(module('htmlTemplates'));

    var scope;

    beforeEach(inject(function ($rootScope) {
        scope = $rootScope.$new();
    }));

    it('should render correct amount of buttons', inject(function ($compile) {

        //Arrange
        scope.options = [
            {value: 1, label: 'label one'},
            {value: 2, label: 'label two'}
        ];

        scope.model = {
            value: 0
        };

        //Act
        var element = angular.element('<tal-single-select-option-buttons options="options" ng-model="model.value"></tal-single-select-option-buttons>');
        element = $compile(element)(scope);
        scope.$apply();

        //Assert
        var optionButtonElements = element.find('form-radio');
        expect(optionButtonElements.length).toBe(2);

    }));

});
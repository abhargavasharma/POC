
(function (module) {
    'use strict';

    module.directive('talStateStyling', ['SECTION', function (SECTION) {
        return {
            restrict: 'A',
            scope: {
                section: '=',
                iconStyle: '='
            },
            link: function (scope, elem) {
                scope.$watch('section.states', function () {
                    elem.removeClass('completed valid warning error');
                    elem.removeClass('fa fa-check fa-exclamation-circle fa-exclamation-triangle');
                    var iconClass = '';
                    var addedClass = '';
                    var returnCssClasses = '';
                    returnCssClasses = returnCssClasses.concat(scope.section.selected ? ' selected' : '');
                    if (scope.section.states[SECTION.STATUS.VALID])
                    {
                        addedClass = SECTION.STATUS.VALID;
                        if(scope.iconStyle) {
                            iconClass =  '';
                        }
                    }
                    if (scope.section.states[SECTION.STATUS.COMPLETED])
                    {
                        addedClass = SECTION.STATUS.COMPLETED;
                        if(scope.iconStyle){
                            iconClass = 'fa fa-check';
                        }
                    }
                    if (scope.section.states[SECTION.STATUS.WARNING])
                    {
                        addedClass = SECTION.STATUS.WARNING;
                        if(scope.iconStyle) {
                            iconClass =  'fa fa-exclamation-triangle';
                        }
                    }
                    if (scope.section.states[SECTION.STATUS.ERROR])
                    {
                        addedClass = SECTION.STATUS.ERROR;
                        if(scope.iconStyle) {
                            iconClass =  'fa fa-exclamation-triangle';
                        }
                    }
                    if(scope.iconStyle){
                        elem.addClass(iconClass);
                        return;
                    }
                    elem.addClass(returnCssClasses.concat(addedClass));
                }, true);
            }
        };
    }]);
})(angular.module('salesPortalApp'));
(function (module) {
    'use strict';
/*
    I'm a pretty awesome directive, attach me an element above any inputs and I'll perform your function when the user
    leaves any child field and/or changes the value of that field.
    FYI, I also piggy-back off the tal-server-validate directive, so for best results your inputs should also use that as well
 */
    module.directive('talInlineSubmit', function ($q, $timeout, talFormModelStateService) {
        return {
            restrict: 'A',
            scope: {
                talInlineSubmit: '&'
            },
            link: function (scope, elem, attrs) {

                if (attrs.disableInlineSubmit === 'true') {
                    return;
                }

                scope.checkDirty = true;

                var inputElems = elem.find('input');
                var selectElems = elem.find('select');
                var textAreaElems = elem.find('textarea');

                var submitAndMonitor = function(evt) {

                    var thisElem = angular.element(evt.target);

                    var serverValidateKey = thisElem.attr('tal-server-validate') || thisElem.attr('inline-submit-use-server-validate-key');

                    var ngModel = talFormModelStateService.getModelStateForKey(serverValidateKey);
                    if (scope.checkDirty && !(ngModel && ngModel.$dirty)) {
                        return;
                    }

                    $timeout(scope.talInlineSubmit); //Kinda hacky but fixes issue where event fires and model state is not updated in time
                };

                //Attach trigger event to input elements
                _.each(inputElems, function(inputElem) {
                    var $elem = angular.element(inputElem);
                    var eventName;

                    var customTrigger = $elem.attr('inline-submit-custom-trigger');

                    if (customTrigger) {
                        eventName = customTrigger;
                    } else {
                        switch (inputElem.type) {
                            case 'hidden':
                                break;
                            case 'radio':
                            case 'checkbox':
                                eventName = 'change';
                                scope.checkDirty = false; //radio and checkbox change event means it's dirty anyway
                                break;
                            default:
                                eventName = 'blur';

                        }
                    }
                    if (eventName) {
                        $elem.on(eventName, submitAndMonitor);
                    }
                });

                selectElems.on('change', submitAndMonitor); //Attach trigger event to select elements
                textAreaElems.on('blur', submitAndMonitor); //Attach trigger event to textarea elements

            }
        };
    });
})(angular.module('salesPortalApp'));
(function (module) {
    'use strict';

    module.directive('talOccupationSearch', function () {
        return {
            require    : ['^ngModel'],
            templateUrl: '/client/appCustomerPortal/components/occupationSearch/occupationSearch.component.template.html',
            restrict   : 'E',
            scope      : {
                serverValidate: '@',
                onChange      : '&',
                selectedTitle : '=',
                inlineSubmit  : '&',
                noInlineSubmit: '@',
                talId         : '@'
            },
            controller : 'talOccupationSearchController',
            link       : function (scope, element, attrs, ngModelCtrl) {
                scope.element = element;
                scope.ngModelCtrl = ngModelCtrl[0];
                function keydown(event, index) {
                    if (event.keyCode === 40 || event.keyCode === 38) {
                        event.preventDefault();
                        var items = element.find('li'),
                            newIndex = (index + (event.keyCode - 39)) % items.length;

                        if (newIndex < 0) {
                            element.find('input').focus();
                        } else if(items[newIndex]){
                            items[newIndex].focus();
                        }
                    }
                }

                scope.occupation = scope.occupation || {};
                scope.occupation.keydown = keydown;
            }
        };
    });

    function talOccupationSearchController($scope, talOccupationSearchService, $sce, $timeout) {
        var timer = null, answerOnLoad;

        var DEFAULT_OCCUPATION_TEXT = '';

        $scope.occupation = $scope.occupation || {};
        $scope.occupation.searchAnswers = [];
        $scope.occupation.searchPhrase = '';
        $scope.occupation.selectedAnswer = null;
        $scope.occupation.selectedIndustry = DEFAULT_OCCUPATION_TEXT;
        $scope.occupation.allowSearch = true;

        function resetAnswer() {
            $scope.occupation.selectedIndustry = DEFAULT_OCCUPATION_TEXT;
            $scope.occupation.allowSearch = true;
            $scope.occupation.selectedAnswer = null;

            var value = $scope.ngModelCtrl.$modelValue || {};
            value.occupationCode = null;
            value.occupationTitle = null;
            value.industryCode = null;
            value.industryTitle = null;
            $scope.ngModelCtrl.$setViewValue(value);
        }

        function clearSearchResults(delay) {
            $timeout(function () {
                $scope.occupation.searchAnswers = [];
            }, delay);
        }

        function onOccupationSelected(answer) {
            $scope.occupation.selectedAnswer = answer;
            $scope.occupation.searchPhrase = answer.text;
            $scope.occupation.selectedIndustry = answer.parentText;
            clearSearchResults(0);
            $scope.occupation.allowSearch = false;

            var value = $scope.ngModelCtrl.$modelValue || {};
            value.occupationCode = answer.responseId;
            value.occupationTitle = answer.text;
            value.industryCode = answer.parentResponseId;
            value.industryTitle = answer.parentText;
            $scope.ngModelCtrl.$setViewValue(value);

            if ($scope.onChange) {
                $timeout(function () {
                    $scope.onChange();
                    var inputElement = $scope.element.find('input');
                    inputElement.triggerHandler('occupation-changed');
                });
            }
        }

        function burstDelay(fn, delay) {
            return function () {
                var context = this, args = arguments;
                clearTimeout(timer);
                timer = setTimeout(function () {
                    fn.apply(context, args);
                }, delay);
            };
        }

        function highlight(text, search) {
            if (!search || !text) {
                return $sce.trustAsHtml(text);
            }
            return $sce.trustAsHtml(text.replace(new RegExp(search, 'gi'), '<span class="highlight">$&</span>'));
        }

        function updateAnswerList(query) {
            if (answerOnLoad) {
                var matchingAnswer = _.first(_.filter($scope.occupation.searchAnswers, function (sa) {
                    return sa.responseId === answerOnLoad;
                }));
                if (matchingAnswer) {
                    onOccupationSelected(matchingAnswer);
                    answerOnLoad = '';
                    return;
                }
            }
            answerOnLoad = '';

            _.each($scope.occupation.searchAnswers, function (item) {
                if ($scope.occupation.searchAnswers.length <= 2 && item.parentText.toUpperCase() === query.toUpperCase()) {
                    // exact match and only 1 option other than not listed available
                    onOccupationSelected(item);
                }
                item.parentText = highlight(item.parentText, query);
            });
        }

        function onTextChange(newValue, oldValue) {
            if ($scope.occupation.searchPhrase === oldValue) {
                return;
            }

            if (!$scope.occupation.allowSearch) {
                $scope.occupation.allowSearch = true;
                return;
            }

            resetAnswer();

            var query = String(newValue);
            if (newValue && newValue !== '') {
                burstDelay(function () {
                    talOccupationSearchService.search(query, function (result) {
                        $scope.occupation.searchAnswers = result;
                        updateAnswerList(query);
                    });
                }, 150)();

            }
        }

        $scope.occupation.hideResults = clearSearchResults;
        $scope.occupation.onChange = onOccupationSelected;
        $scope.$watch(function () {
            return $scope.occupation.searchPhrase;
        }, onTextChange);

        $scope.$on('form-data-loaded', function (event, ratingFactors) {
            if (ratingFactors.occupationTitle) {
                answerOnLoad = ratingFactors.occupationCode;
                $scope.occupation.searchPhrase = ratingFactors.occupationTitle;
            } else {
                resetAnswer();
                $scope.occupation.searchPhrase = '';
            }
        });
    }

    module.controller('talOccupationSearchController', talOccupationSearchController);
    talOccupationSearchController.$inject = ['$scope', 'talOccupationSearchService', '$sce', '$timeout'];

})(angular.module('appCustomerPortal'));
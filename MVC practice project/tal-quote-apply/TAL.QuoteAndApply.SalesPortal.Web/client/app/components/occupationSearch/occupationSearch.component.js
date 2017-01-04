(function (module) {
    'use strict';

    module.directive('talOccupationSearch', function () {
        return {
            require: ['^ngModel'],
            templateUrl: '/client/app/components/occupationSearch/occupationSearch.component.template.html',
            restrict: 'E',
            scope: {
                serverValidate:'@',
                onChange: '&',
                selectedTitle: '=',
                inlineSubmit: '&',
                noInlineSubmit: '@'
            },
            controller: 'talOccupationSearchController',
            link: function(scope, element, attrs, ngModelCtrl) {
                scope.element = element;
                scope.ngModelCtrl = ngModelCtrl[0];
                function keydown(event, index) {
                    if (event.keyCode === 40 || event.keyCode === 38) {
                        event.preventDefault();
                        var items = element.find('li'),
                            newIndex = (index + (event.keyCode - 39)) % items.length;

                        if (newIndex < 0) {
                            element.find('input').focus();
                        } else {
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
        $scope.occupation = $scope.occupation || {};
        $scope.occupation.searchAnswers = [];
        $scope.occupation.searchPhrase = '';
        $scope.occupation.selectedAnswer = null;
        $scope.occupation.selectedIndustry = 'Please Select an occupation';
        $scope.occupation.allowSearch = true;

        function resetAnswer() {
            $scope.occupation.selectedIndustry = 'Please Select an occupation';
            $scope.occupation.allowSearch = true;
            $scope.occupation.selectedAnswer = null;
        }

        function clearSearchResults(delay) {
                $timeout(function() {
                    $scope.occupation.searchAnswers = [];
            }, delay);
        }

        function onOccupationSelected(answer) {
            $scope.occupation.selectedAnswer = answer;
            $scope.occupation.searchPhrase = answer.text;
            $scope.occupation.selectedIndustry = answer.parentText;
            clearSearchResults(0);
            $scope.occupation.allowSearch = false;

            if($scope.onChange) {
                $timeout(function () {
                    var value = $scope.ngModelCtrl.$modelValue || {};
                    value.occupationCode = answer.responseId;
                    value.occupationTitle = answer.text;
                    value.industryCode = answer.parentResponseId;
                    value.industryTitle = answer.parentText;
                    $scope.ngModelCtrl.$setViewValue(value);
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
                var matchingAnswer = _.first(_.filter($scope.occupation.searchAnswers, function(sa) {
                    return (sa.responseId === answerOnLoad.responseId && sa.parentResponseId === answerOnLoad.parentResponseId);
                }));
                if (matchingAnswer) {
                    onOccupationSelected(matchingAnswer);
                    answerOnLoad = null;
                    return;
                }
            }
            answerOnLoad = null;

            _.each($scope.occupation.searchAnswers, function (item) {
                if ($scope.occupation.searchAnswers.length <= 2 && item.displayText.toUpperCase() === query.toUpperCase()) {
                    // exact match and only 1 option other than not listed available
                    onOccupationSelected(item);
                }
                item.displayText = highlight(item.displayText, query);
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
            if (ratingFactors.occupationTitle ) {

                //this is to ensure when a section reloads after already existing that the new next search doesn't include the onload answer
                if(ratingFactors.occupationTitle !== $scope.occupation.searchPhrase) {
                    answerOnLoad = {
                        responseId: ratingFactors.occupationCode,
                        parentResponseId: ratingFactors.industryCode
                    };
                    $scope.occupation.searchPhrase = ratingFactors.occupationTitle;
                }
            } else {
                resetAnswer();
                $scope.occupation.searchPhrase = '';
            }
        });
    }

    module.controller('talOccupationSearchController', talOccupationSearchController);
    talOccupationSearchController.$inject = ['$scope', 'talOccupationSearchService', '$sce', '$timeout'];

})(angular.module('salesPortalApp'));
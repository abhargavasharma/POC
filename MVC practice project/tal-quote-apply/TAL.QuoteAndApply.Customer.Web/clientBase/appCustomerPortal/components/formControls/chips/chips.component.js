(function(module) {
    'use strict';
	module.factory('talChipsService', ['$log', '$timeout', function($log, $timeout) {
			return {
				
				helper: function(scope, collection, active, rule) {
					if (active === -1 && collection.length > 0) {
						collection[0].active = true;
					} else if (collection.length > 0) {
						if (collection[active+rule]) {
							collection[active].active = false;
							collection[active+rule].active = true;
						} else {
							collection[active].active = false;
							var index = rule === 1 ? 0 : (collection.length-1);
							collection[index].active = true;
						}
					}
					scope.$apply();

					// focus tracks the active item.
					$timeout(function() {
						var $item = document.querySelector('#tal-chips-list').querySelector('.tal-chips-list-item.active');
						if($item){
							$item.focus();
						}
					});
				},
				nextActive: function(scope, collection, active) {
					$log.log('tal-chips: next');
					this.helper(scope, collection, active, 1);
				},
				prevActive: function(scope, collection, active) {
					$log.log('tal-chips: prev');
					this.helper(scope, collection, active, -1);
				}
			};
		}
	])
	.directive('talChips', ['$log','$compile','$timeout', '$document', 'talChipsService', function($log, $compile, $timeout, $document, talChipsService) {
		return {
			restrict: 'E',
			replace:true,
			templateUrl: '/client/appCustomerPortal/components/formControls/chips/chips.template.html',
			require: '?ngModel',
			scope: {
				collection: '=',
				ngModel: '=',
				text: '@',
				talItem: '@',
				talTitle: '@',
				disabled: '=',
				placeholder: '@',
				unique: '=',
				singleOnly: '=',
				allowFreeEntry: '=',
				inputType: '@'
			},
			link: function (scope, element) {

				if(scope.inputType === null || scope.inputType === undefined){
					scope.inputType = 'text';
				}

				//todo: add limitTo: 2
				var templateChipList = [
					'<div id="tal-chips-list" ng-show="true" ng-cloak>',
						'<div ng-repeat="item in (filteredCollection = (innerCollection | filter:chipsText))" ' +
							'class="tal-chips-list-item" ng-click=addToInput(item) ng-class="{active: item.active, selected: item.isSelected}">',
							'<div class="tal-chips-item-wrapper">',
								'<span class="help-icon" ng-if="item.helpText" ng-click="toggleHelp($event, item)"></span>',
								'<span class="tal-chips-title">{{item[talTitle]}}</span>',
								'<span class="help-text" ng-if="item.helpText" ng-show="item.isHelpVisible">{{item.helpText}}</span>',
							'</div>',
						'</div>',
					'</div>'
				].join('');

				var compileAndShowList = function () {
					scope.$apply(function() {
						var	list = angular.element(templateChipList);
						$compile(list)(scope);
						$timeout(function() {
							scope.removeList();
							element.append(list);
						});
					});
				};

				scope.toggleHelp = function ($event, item) {
					$event.stopPropagation();

					item.isHelpVisible = !item.isHelpVisible;
				};

				scope.ytop = '10px';

				scope.$watch('disabled',function() {
		            if (scope.disabled) {
		            	element[0].className += ' tal-chips-disabled';
		            } else {
		            	element[0].className = element[0].className.replace(' tal-chips-disabled', '');
		            }
		        });

				scope.innerCollection = scope.collection.map(function(item) {
					if (!item[scope.talTitle]) { 
						return; 
					}

					item.active = false;

					return item;
				});

				element.find('input').bind('click', function () {
					//$log.debug('tal-chips: click');
					compileAndShowList();
				});

				element.bind('input', function (event) {
					//$log.log('tal-chips: input event');

					scope.clearActive();

					if (event.target.value === scope.prevSearchValue || scope.prevSearchValue === undefined) {
					    scope.prevSearchValue = event.target.value;
						return;
					}

					compileAndShowList();

					scope.prevSearchValue = event.target.value;
				});
			
				$document.bind('click', function() {
					//$log.log('tal-chips: $document click');
					scope.clearActive();
					if (scope.chipsText) {
						scope.chipsText[scope.talTitle] = '';
					}
					scope.removeList();
					scope.$apply();
				});

				element.bind('click', function(evt) {
					//$log.log('tal-chips: element click');
					if (scope.disabled) {
						evt.stopPropagation();
						evt.stopImmediatePropagation();
						evt.preventDefault();
						element.find('input')[0].disabled = true;
						return;
					}
					element.find('input')[0].disabled = false;
					evt.stopPropagation();
					element[0].querySelector('.tal-chips-input').focus();
				});

				scope.checkDisabled = function(evt) {
					// $log.log('tal-chips: checkDisabled ' + scope.disabled);
					if (scope.disabled) {
						evt.stopPropagation();
						evt.stopImmediatePropagation();
						evt.preventDefault();
						return;
					}
				};

				scope.removeList = function() {
					// $log.log('tal-chips: removeList');
					scope.innerCollection.forEach(function(item) {
						if (item.active) {
							item.active = false;
						}
					});
					var chipsList = element[0].querySelector('#tal-chips-list');
					if (chipsList) {
						chipsList.parentNode.removeChild(chipsList);
					}
				};

				scope.checkPresenceInArray = function(obj) {
					// $log.log('tal-chips: checkPresenceInArray');
					var result = scope.ngModel.some(function(item) {
						var exactMatch = true;
						for (var i in item) {
							if (JSON.stringify(item[i]) !== JSON.stringify(obj[i])) {
								exactMatch = false;
							}
						}
						if (exactMatch) {
							return true;
						}

					});
					return result;
				};

				scope.addToInput = function(item) {
					//$log.log('tal-chips: addToInput');
					if (scope.unique && !scope.checkPresenceInArray(item) || !scope.unique) {
						item.isSelected = true;
						scope.ngModel.push(item);
					}

					if(scope.chipsText){
						scope.chipsText[scope.talTitle] = '';
					}

					this.removeList();
					element[0].querySelector('.tal-chips-input').focus();
				};

				element.bind('keydown', function(e) {
					// $log.log('tal-chips: keydown keycode:' + e.keyCode);
					var chipsList = element[0].querySelector('#tal-chips-list');
					var active = -1;

					if (!chipsList) { return; }

					scope.filteredCollection.forEach(function(item, index) {
						if (item.active) {
							active = index;
						}
					});

					switch(e.keyCode) {
						case 40:
							e.preventDefault();
							talChipsService.nextActive(scope, scope.filteredCollection, active);
							break;	
						case 38:
							e.preventDefault();
							talChipsService.prevActive(scope, scope.filteredCollection, active);
							break;
						case 13:	//enter
							if (active !== -1) {
								scope.addToInput(scope.filteredCollection[active]);
								scope.removeList();
								e.target.style.width = 20;
								scope.$apply();
							} else if (scope.chipsText[scope.talTitle] ) {
								if (scope.allowFreeEntry) {
									var item = {};
									item[scope.talTitle] = scope.chipsText[scope.talTitle]; 
									scope.addToInput(item);
									e.target.style.width = 20;
									scope.$apply();
								}
							}
							break;
						default:
							break;
					}
					
				});

				scope.deleteChips = function(index) {
					//$log.log('tal-chips: deleteChips: ' + index);
					var item = scope.ngModel[index];
					if(item){
						item.isSelected = false;
					}

					scope.ngModel.splice(index, 1);
					scope.clearActive();
					scope.removeList();
				};

				scope.isSingleOnlySelected = function () {
					// $log.log('tal-chips: isSingleOnlySelected');
					if(!scope.singleOnly){
						return false;
					}

					return scope.ngModel.length;
				};

				scope.clearPrev = function(event) {
					// $log.log('tal-chips: clearPrev');
					if (event.keyCode === 8 && event.target.value === '' && scope.ngModel.length !== 0) {
						scope.deleteChips(scope.ngModel.length -1);
					}
					if (scope.chipsText) {
						var length = scope.chipsText[scope.talTitle].length * 15 + 15;
						event.target.style.width = length ? length : 20;
					}
					return true;
				};

				scope.closeActive = function(event) {
					// $log.log('tal-chips: closeActive');
					if (event.currentTarget.hasChildNodes()) {
						scope.clearActiveChildren(event.currentTarget);
					}
				};

				scope.clearActive = function() {
					// $log.log('tal-chips: clearActive');
					var chipsActive = element[0].querySelector('.tal-chips-active');
					scope.clearActiveChildren(chipsActive);
					
				};

				scope.clearActiveChildren = function(active) {
					// $log.log('tal-chips: clearActiveChildren');
					while (active.firstChild) {
					    active.removeChild(active.firstChild);
					}
				};
			}
		};

	}
]);

})(angular.module('appCustomerPortal'));

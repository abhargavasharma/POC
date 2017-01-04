(function(module){
    'use strict';
    module.directive('formDob', function () {
        return {
            templateUrl: '/client/appCustomerPortal/components/formControls/formDoB/formDoB.template.html',
            restrict: 'E',
            scope: {
                ngModel: '=',
                id:'@talId',
                name:'@talName'
            },
            transclude:true,
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'formDoB',
            require: 'ngModel',
            link: function(scope, elem, attrs, ngModelCtrl) {

               var setValues = function(newValue){
                  if(newValue === undefined){
                    scope.ctrl.day = '00';
                    scope.ctrl.month = '00';
                    scope.ctrl.year = '0000';
                  } else {
                    scope.ctrl.day = newValue.substring(0,2);
                    scope.ctrl.month = newValue.substring(3,5);
                    scope.ctrl.year = newValue.substring(6,10);
                  }
                    
               };
               var compileDate = function(){
                    var tmp = [];
                    tmp.push(scope.ctrl.day);
                    tmp.push(scope.ctrl.month);
                    tmp.push(scope.ctrl.year);

                    return tmp.join('/');
               };
               scope.validateDate = function(date){
                    var pattern = new RegExp(/^\d{2}\/\d{2}\/\d{4}$/);
                    return pattern.test(date);
               };

               var updateModel = function(){
                    var tmp = compileDate();
                    if(scope.validateDate(tmp)){
                        ngModelCtrl.$setViewValue(tmp);
                    }
               };
               scope.$watch('ctrl.day', function(){
                    updateModel();
               });
               scope.$watch('ctrl.month', function(){
                    updateModel();
               });
               scope.$watch('ctrl.year', function(){
                    updateModel();
               });
               setValues(scope.ctrl.ngModel);

               scope.$watch(
                    function(){
                        return ngModelCtrl.$modelValue;
                    }, function(newValue){
                        if(scope.validateDate(newValue)){
                            setValues(newValue);
                        }
                        
                    }, true);
            
      }
    };
  });

  function formDoB() {
    
  }

  module.controller('formDoB', formDoB );
  formDoB.$inject = [];

})(angular.module('appCustomerPortal'));

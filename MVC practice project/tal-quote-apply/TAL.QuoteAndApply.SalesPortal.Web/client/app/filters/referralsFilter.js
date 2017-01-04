
(function(module){
    'use strict';
    module.filter('talReferralsFilter', function () {

        function parseDate(dateTime) {

            var date = new Date(dateTime);

            var dd = date.getDate();
            var mm = date.getMonth()+1; //January is 0!
            var yyyy = date.getFullYear();
            if(dd<10){
                dd='0'+dd;
            }
            if(mm<10){
                mm='0'+mm;
            }
            var returnDate = yyyy+'-'+mm+'-'+dd;

            var hours = date.getHours();
            if(hours<10){
                hours='0'+hours;
            }
            var minutes = '0' + date.getMinutes();
            var seconds = '0' + date.getSeconds();

            var formattedDateTime = returnDate + 'T' + hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2);

            return formattedDateTime;
        }

        return function(items, submitModel) {

            var hasCorrectPlans = function(plans){
                var planFiltersSelected = 0;

                if(submitModel.DTH){
                    planFiltersSelected++;

                    if(_.indexOf(plans, 'DTH') > -1){
                        return true;
                    }
                }
                if(submitModel.TPS) {
                    planFiltersSelected++;

                    if(_.indexOf(plans, 'TPS') > -1){
                        return true;
                    }
                }
                if(submitModel.TRS) {
                    planFiltersSelected++;

                    if(_.indexOf(plans, 'TRS') > -1){
                        return true;
                    }
                }
                if(submitModel.IP) {
                    planFiltersSelected++;

                    if(_.indexOf(plans, 'IP') > -1){
                        return true;
                    }
                }

                return plans.length === 0 && planFiltersSelected === 0;
            };

            var hasCorrectUnderwriter = function(underwriter){
                if(submitModel.assignedTo === '' || submitModel.assignedTo === null || underwriter === submitModel.assignedTo){
                    return true;
                }
                return false;
            };

            var hasCorrectStates = function(state){
                var hasState = true;
                if(!submitModel.Unresolved && state === 'Unresolved'){
                    hasState = false;
                }
                if(!submitModel.Resolved && state === 'Resolved') {
                    hasState = false;
                }
                if(!submitModel.InProgress && state === 'InProgress') {
                    hasState = false;
                }
                return hasState;
            };

            if(!submitModel.startDate){
                submitModel.startDate = new Date('January 01, 1983 00:00:00');
            }
            if(!submitModel.endDate){
                submitModel.endDate = new Date();
            }
            if(items) {
                var df = parseDate(submitModel.startDate.setHours(0, 0, 0, 0));
                var dt = parseDate(submitModel.endDate.setHours(23, 59, 59, 999));
                var result = [];

                for (var i = 0; i < items.length; i++) {
                    var newDate = new Date(items[i].createdTS);
                    var tf = parseDate(newDate);
                    if (tf > df && tf < dt && hasCorrectPlans(items[i].plans) && hasCorrectUnderwriter(items[i].assignedTo) && hasCorrectStates(items[i].state)) {
                        result.push(items[i]);
                    }
                }
                return result;
            }
            return items;
        };
    });

})(angular.module('salesPortalApp'));
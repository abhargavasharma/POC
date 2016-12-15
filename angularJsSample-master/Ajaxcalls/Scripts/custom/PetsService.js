(function () {
    var app = angular.module('app');

    app.factory('PetsServices', function ($http, $q) {
        return {
            getPetsByGender: function () {
                var deferred = $q.defer();

                $http({
                    method: 'GET',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    url: '/Home/getPets'
                }).then(function successCallback(response) {
                    alert('success from service');
                    deferred.resolve(response);
                }, function errorCallback(response) {
                    alert('fail from service');
                    deferred.reject(response);
                });

                return deferred.promise;
            }
        }
    });
})();
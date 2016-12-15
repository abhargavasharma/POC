(function () {


    var app = angular.module('app');

    app.controller('petsController1', petsController1);

    petsController1.$inject = ['PetsServices'];

    function petsController1(PetsServices) {
        var vm = this;
        vm.pets1 = {
            gender: "",
            cats: {}
        };

        var promise = PetsServices.getPetsByGender();
        promise.then(function (response) {
            vm.pets1 = response.data;
        });


    }

})();
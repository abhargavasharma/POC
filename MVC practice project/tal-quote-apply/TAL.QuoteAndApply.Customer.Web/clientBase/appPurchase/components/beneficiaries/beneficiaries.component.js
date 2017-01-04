(function (module) {
    'use strict';
    module.directive('talBeneficiaries', function () {
        return {
            templateUrl: '/client/appPurchase/components/beneficiaries/beneficiaries.template.html',
            restrict: 'E',
            scope: {
                riskId: '=',
                beneficiaries: '=',
                errorModel: '=',
                personalDetails: '='
            },
            bindToController: true,
            controllerAs: 'ctrl',
            controller: 'talBeneficiariesController'
        };
    });

    function talBeneficiariesController(talCustomerPortalApiService,FORM,ngDialog) {

        var ctrl = this;

        ctrl.titles = FORM.PERSONAL_DETAILS.TITLES;
        ctrl.states = FORM.ADDRESS.STATES;
        ctrl.maxNumberOfBeneficiaries = 5;
        var addBeneficiaryText = 'Add a beneficiary';

        ctrl.addBeneficiaryText = addBeneficiaryText;

        ctrl.canAddBeneficiary = function () {
            return ctrl.beneficiaries && ctrl.beneficiaries.length < ctrl.maxNumberOfBeneficiaries;
        };

        ctrl.setBeneficiaryButtonText = function () {
            ctrl.addBeneficiaryText = (ctrl.beneficiariesExist && ctrl.canAddBeneficiary()) ? 'Add another beneficiary' : addBeneficiaryText;
        };

        ctrl.blankBeneficiary = {
            firstName: null,
            surname: null,
            dateOfBirth: null,
            address: null,
            relationshipToRisk: null,
            share: null,
            id: 0,
            copyAddressChecked: false
        };

        ctrl.pushBeneficiary = function () {
            this.beneficiaryToPush = _.cloneDeep(this.blankBeneficiary);
            this.beneficiaries.push(this.beneficiaryToPush);
        };

        ctrl.addBeneficiary = function () {
            this.pushBeneficiary();
            this.beneficiariesExist = true;
            ctrl.setBeneficiaryButtonText();
        };

        ctrl.removeBeneficiary = function (indexOfBeneficiary, beneficiaryId) {
            var dialog = ngDialog.open({
                templateUrl: '/client/appPurchase/components/beneficiaries/modal/confirmRemoval.template.html',
                controllerAs: 'ctrl'
            });

            dialog.closePromise.then(function (data) {
                if (data.value === true) {
                    ctrl.removeBeneficiaryAction(indexOfBeneficiary, beneficiaryId);
                }
            });
        };

        ctrl.removeBeneficiaryAction = function (indexOfBeneficiary, beneficiaryId) {
            this.beneficiaries.splice(indexOfBeneficiary, 1);
            if (this.beneficiaries.length < 1) {
                this.beneficiariesExist = false;
            }
            if (beneficiaryId) {
                this.loadingPromise = talCustomerPortalApiService.removeBeneficiary(ctrl.riskId, beneficiaryId).then(function () {
                    //onActivation();
                });
            }
            ctrl.setBeneficiaryButtonText();
        };

        ctrl.changeAddressWatch = function(indexOfBeneficiary) {
            ctrl.beneficiaries[indexOfBeneficiary].copyAddressChecked = false;
        };

        ctrl.copyAddress = function (beneficiary) {
            beneficiary.address = beneficiary.copyAddressChecked ? ctrl.personalDetails.residentialAddress : '';
            beneficiary.postcode = beneficiary.copyAddressChecked ? ctrl.personalDetails.postcode : '';
            beneficiary.state = beneficiary.copyAddressChecked ? ctrl.personalDetails.state : undefined;
            beneficiary.suburb = beneficiary.copyAddressChecked ? ctrl.personalDetails.suburb : '';
        };

        ctrl.getRelationshipDesc = function (relationshipId) {
            if (relationshipId) {
                return _.find(ctrl.relationships, { id: relationshipId }).description;
            }
            return 'Please select';
        };

        talCustomerPortalApiService.getBenefitRelationships().then(function (response) {
            ctrl.relationships = response.data;
        });

        talCustomerPortalApiService.getMaxBeneficiaries().then(function (response) {
            ctrl.maxNumberOfBeneficiaries = response.data;
        });
    }

    module.controller('talBeneficiariesController', talBeneficiariesController);
    talBeneficiariesController.$inject = ['talCustomerPortalApiService', 'FORM', 'ngDialog'];

})(angular.module('appPurchase'));


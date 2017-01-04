
(function (module) {
    'use strict';
    module.directive('talRiskBeneficiariesSection', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskBeneficiariesSection/riskBeneficiariesSection.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk: '=',
                quoteReferenceNumber: '=',
                readOnly: '='
            },
            controller: 'talRiskBeneficiariesSectionController'
        };
    });

    function talRiskBeneficiariesSectionController($scope, $timeout, $uibModal, FORM, talSalesPortalApiService, talFormModelStateService, talPolicyEditSectionsService, talPolicySectionsNotificationService) {
        $scope.titles = FORM.PERSONAL_DETAILS.TITLES;
        $scope.states = FORM.ADDRESS.STATES;
        $scope.lpr = false;

        var maxBeneficiaries = 5; //default value for tal

        function removeBeneficiary(beneficiary) {
            var modalInstance = $uibModal.open({
                animation: $scope.animationsEnabled,
                templateUrl: '/client/app/components/policy/risks/riskBeneficiariesSection/modal/removeBeneficiary.modal.html',
                controller: 'beneficiariesModalController',
                size: 'md'
            });

            modalInstance.result.then(function() {
                //remove beneficiary:
                var removedIndex = $scope.beneficiaries.indexOf(beneficiary);
                $scope.beneficiaries.splice(removedIndex, 1);

                if ($scope.beneficiaries.length === 0) {
                    $scope.beneficiaries.push({ share: 100 });
                    $scope.section.isModelValid = true;
                }

                if (beneficiary.id > 0) {
                    $scope.updatingPromise = talSalesPortalApiService.removeBeneficiary($scope.quoteReferenceNumber, $scope.risk.riskId, beneficiary.id);
                }
                strictlyValidateForm();
            });
        }

        function updateLpr() {
            if ($scope.lpr === true) {
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: '/client/app/components/policy/risks/riskBeneficiariesSection/modal/lprConfirmation.modal.html',
                    controller: 'beneficiariesModalController',
                    size: 'md'
                });

                modalInstance.result.then(function () {
                    $scope.loadingPromise = talSalesPortalApiService.updateBeneficiaryOptions($scope.quoteReferenceNumber, $scope.risk.riskId, { nominateLpr: $scope.lpr }).then(function (response) {
                        $scope.section.isCompleted = true;
                        $scope.section.isModelValid = true;
                        onActivation();
                        talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section, response.data.warnings);
                        strictlyValidateForm();
                    });
                }, function () {
                    $scope.lpr = false;
                    $scope.section.isCompleted = false;
                });
            } else {
                $scope.section.isCompleted = false;
                $scope.loadingPromise = talSalesPortalApiService.updateBeneficiaryOptions($scope.quoteReferenceNumber, $scope.risk.riskId, { nominateLpr: $scope.lpr }).then(function (response) {
                    if (response.data) {
                        talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section, response.data.warnings);
                    }
                    softlyValidateForm($scope.submitForm);
                }).catch(function () {
                    talPolicyEditSectionsService.setSectionStatesOnError($scope.section);
                });
            }

        }

        function onActivation() {
            
            $scope.loadingPromise = talSalesPortalApiService.getRiskAllBeneficiaries($scope.quoteReferenceNumber, $scope.risk.riskId).then(function (response) {
                $scope.beneficiaries = response.data;

                if ($scope.beneficiaries.length === 0) {
                    $scope.beneficiaries.push({ share: 100 });
                    $scope.section.isModelValid = true;
                } else {
                    $scope.submitForm(true);
                }
                $scope.loadingPromise = talSalesPortalApiService.getBeneficiaryOptions($scope.quoteReferenceNumber, $scope.risk.riskId).then(function (response) {
                    $scope.lpr = response.data.nominateLpr;
                    if ($scope.lpr) {
                        talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section);
                        $scope.section.isCompleted = true;
                    }
                });
            });
        }

        function softlyValidateForm(hideUiUpdate) {
            var updatingPromise = talSalesPortalApiService
                .updateRiskBeneficiariesDetails($scope.quoteReferenceNumber, $scope.risk.riskId, $scope.beneficiaries)
                .then(
                    function(response) {
                        $scope.beneficiaries = response.data;
                        $scope.section.isModelValid = true;
                        $scope.section.isCompleted = _.every(response.data, function(b) { return b.isCompleted; });
                        talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section, response.data.warnings);
                    },
                    function(response) {
                        $scope.section.isModelValid = false;
                        $scope.section.isCompleted = false;
                        talPolicyEditSectionsService.setSectionStatesOnError($scope.section);
                        var modelStateErrors = response.data;
                        talFormModelStateService.updateModelState(modelStateErrors, $scope);
                        throw response;
                    });

            if (!hideUiUpdate) {
                $scope.updatingPromise = updatingPromise;
            }
        }

        function strictlyValidateForm() {
            $scope.updatingPromise = talSalesPortalApiService.validateBeneficiaries($scope.quoteReferenceNumber, $scope.risk.riskId).then(function (response) {
                // success
                $scope.section.isModelValid = true;
                $scope.section.isCompleted = true;
                talFormModelStateService.updateModelState({}, $scope);
                talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section, response.data.warnings);
            }, function (response) {
                $scope.section.isModelValid = false;
                talPolicyEditSectionsService.setSectionStatesOnError($scope.section);
                var modelStateErrors = response.data;
                talFormModelStateService.updateModelState(modelStateErrors, $scope);
                throw response;
            });
        }

        function copyBeneficiaryAddress(index, sourceBeneficiary, form) {
            $scope.beneficiaries[index].address = sourceBeneficiary.address;
            $scope.beneficiaries[index].suburb = sourceBeneficiary.suburb;
            $scope.beneficiaries[index].state = sourceBeneficiary.state;
            $scope.beneficiaries[index].postcode = sourceBeneficiary.postcode;
            $scope.beneficiaries[index].phoneNumber = sourceBeneficiary.phoneNumber;
            $scope.beneficiaries[index].surname = sourceBeneficiary.surname;

            softlyValidateForm();
            highlightCopiedFields(form);
        }

        function copyRiskAddress(index, form) {
            $scope.loadingPromise = talSalesPortalApiService.getPolicyOwnerDetails($scope.quoteReferenceNumber).then(function (response) {
                var personalDetails = response.data;
                $scope.beneficiaries[index].address = personalDetails.address;
                $scope.beneficiaries[index].suburb = personalDetails.suburb;
                $scope.beneficiaries[index].state = personalDetails.state;
                $scope.beneficiaries[index].postcode = personalDetails.postcode;
                $scope.beneficiaries[index].phoneNumber = getPhoneNumber(personalDetails.mobileNumber, personalDetails.homeNumber);
                $scope.beneficiaries[index].surname = personalDetails.surname;

                softlyValidateForm();
                highlightCopiedFields(form);
            });
        }

        function getPhoneNumber(mobileNumber, homeNumber) {
            var phoneNumber;
            if (mobileNumber !== null) {
                phoneNumber = mobileNumber;
            }
            else {
                phoneNumber = homeNumber;
            }
            return phoneNumber;
        }

        function highlightCopiedFields(form) {
            form.highlight = true;
            $timeout(function () {
                form.highlight = false;
            }, 1000);
        }

        function addBeneficiary() {
            $scope.beneficiaries.push({ share: 50 });
        }

        function canAddBeneficiary() {
            return $scope.beneficiaries && $scope.beneficiaries.length < maxBeneficiaries;
        }

        talPolicySectionsNotificationService.registerInitialisationChangeEvent(function () {
            onActivation();
        });

        $scope.copyRiskAddress = copyRiskAddress;
        $scope.copyBeneficiaryAddress = copyBeneficiaryAddress;
        $scope.onLprChange = updateLpr;
        $scope.submitForm = softlyValidateForm;
        $scope.addBeneficiary = addBeneficiary;
        $scope.canAddBeneficiary = canAddBeneficiary;
        $scope.validateForm = strictlyValidateForm;
        $scope.removeBeneficiary = removeBeneficiary;
        $scope.section.onActivationEvent = onActivation;


        talSalesPortalApiService.getBenefitRelationships().then(function (response) {
            $scope.relationships = response.data;
        });

        talSalesPortalApiService.getMaxBeneficiaries($scope.quoteReferenceNumber).then(function (response) {
            maxBeneficiaries = response.data;
        });
    }

    module.controller('talRiskBeneficiariesSectionController', talRiskBeneficiariesSectionController);
    talRiskBeneficiariesSectionController.$inject = ['$scope', '$timeout', '$uibModal', 'FORM', 'talSalesPortalApiService', 'talFormModelStateService', 'talPolicyEditSectionsService', 'talPolicySectionsNotificationService'];

})(angular.module('salesPortalApp'));


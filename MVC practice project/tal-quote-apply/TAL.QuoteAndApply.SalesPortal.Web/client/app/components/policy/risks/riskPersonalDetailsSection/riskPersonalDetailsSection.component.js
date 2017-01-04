
(function(module){
    'use strict';
    module.directive('talRiskPersonalDetailsSection', function () {
        return {
            templateUrl: '/client/app/components/policy/risks/riskPersonalDetailsSection/riskPersonalDetailsSection.template.html',
            restrict: 'E',
            scope: {
                section: '=',
                risk: '=',
                quoteReferenceNumber: '=',
                readOnly: '=',
                ownerType: '=',
                externalRefRequired: '=',
                externalRefLabel: '='
            },
            controller: 'talRiskPersonalDetailsSectionController'
        };
    });

    function talRiskPersonalDetailsSectionController($scope, FORM, talSalesPortalApiService, talFormModelStateService, talPolicySectionsNotificationService, talPolicyEditSectionsService, talLoadingOverlayService, $q, SECTION, $timeout) {
        $scope.isDirty = false;
        $scope.titles = FORM.PERSONAL_DETAILS.TITLES;
        $scope.states = FORM.ADDRESS.STATES;
        $scope.countries = FORM.ADDRESS.COUNTRIES;
        $scope.contactMethods = FORM.CONTACT.CONTACT_METHODS;
        //we need copies of models
        $scope.ownerConsentsModel = JSON.parse(JSON.stringify(FORM.CONSENTS.MODEL));
        $scope.lifeInsuredConsentsModel = JSON.parse(JSON.stringify(FORM.CONSENTS.MODEL));

        $scope.lifeInsuredDncOpen = false;
        $scope.ownerDncOpen = false;

        var updateSectionForOwner = function() {
            $scope.isOwnerSeparateFromLifeInsured = $scope.ownerType === 'SelfManagedSuperFund' || $scope.ownerType === 'SuperannuationFund';
            $scope.isFundNameVisible = $scope.ownerType === 'SelfManagedSuperFund' || $scope.ownerType === 'SuperannuationFund';
            $scope.isFundNameEditable = $scope.ownerType === 'SelfManagedSuperFund';
        };
        var ownerAllDncsSelected = function(){
            return $scope.ownerConsentsModel.dncs.every(function (itm) { return itm.selected; });
        };
        var lifeInsuredAllDncsSelected = function () {
            return $scope.lifeInsuredConsentsModel.dncs.every(function (itm) { return itm.selected; });
        };
        
        var onActivation = function () {
            $scope.loadingPromise = talSalesPortalApiService.getPolicyOwnerDetails($scope.quoteReferenceNumber).then(function (response) {
                $scope.lifeInsuredDetails = {};
                $scope.ownerDetails = response.data;

                updateSectionForOwner();
                
                $scope.section.isCompleted = $scope.ownerDetails.isCompleted;
                $scope.section.isModelValid = $scope.section.isCompleted;

                setWarnings();

                $scope.ownerConsentsModel.expressConsent = response.data.expressConsent;

                _.each(response.data.partyConsents, function (consent) {
                    var dncSelected = _.find($scope.ownerConsentsModel.dncs, function (dnc) {
                        return dnc.name === consent;
                    });
                    if (dncSelected) {
                        dncSelected.selected = true;
                    }
                });

                $scope.areAllDncsForOwnerSelected = ownerAllDncsSelected();

                if ($scope.isOwnerSeparateFromLifeInsured) {
                    $scope.loadingPromise = talSalesPortalApiService.getRiskPersonalDetails($scope.quoteReferenceNumber, $scope.risk.riskId).then(function (response) {
                        $scope.lifeInsuredDetails = response.data;

                        //$scope.section.isCompleted = $scope.personalDetails.isCompleted;
                        //$scope.section.isModelValid = true;

                        $scope.lifeInsuredConsentsModel.expressConsent = response.data.expressConsent;
                    });
                }

                talPolicySectionsNotificationService.onInitialValidation();
            });
        };

        var toggleAllDncs = function (partyConsentsModel, toggleStatus) {
            _.each(partyConsentsModel.dncs, function (itm) { itm.selected = toggleStatus; });
            $scope.dncOptionToggled();
        };

        $scope.toggleAllLifeInsuredDncs = function () {
            toggleAllDncs($scope.lifeInsuredConsentsModel, !$scope.areAllDncsForLifeInsuredSelected);
        };

        $scope.toggleAllOwnerDncs = function() {
            toggleAllDncs($scope.ownerConsentsModel, !$scope.areAllDncsForOwnerSelected);
        };

        $scope.dncOptionToggled = function(){
            $scope.areAllDncsForLifeInsuredSelected = lifeInsuredAllDncsSelected();
            $scope.areAllDncsForOwnerSelected = ownerAllDncsSelected();
            submitForm();
        };

        var submitForm = function(callBack){
            
            $scope.ownerDetails.expressConsent = $scope.ownerConsentsModel.expressConsent;
            $scope.ownerDetails.partyConsents = _.map(_.filter($scope.ownerConsentsModel.dncs, function (n) {
                return n.selected;
            }), 'name');
            
            //update owner details
            $scope.updatingPromise = talSalesPortalApiService.updatePolicyOwnerDetails($scope.quoteReferenceNumber, $scope.ownerDetails)
                .then(function(response) {
                    
                    $scope.section.isCompleted = response.data.isCompleted;
                    $scope.section.isModelValid = $scope.section.isCompleted;
                    
                    talFormModelStateService.updateCleanModelState($scope);

                    $scope.isDirty = false;
                    
                    //if the owner type is SMSF update lifeInsuredDetails:
                    if ($scope.isOwnerSeparateFromLifeInsured) {
                        $scope.lifeInsuredDetails.expressConsent = $scope.lifeInsuredConsentsModel.expressConsent;

                        $scope.updatingPromise = talSalesPortalApiService.updateLifeInsuredDetails($scope.quoteReferenceNumber, $scope.risk.riskId, $scope.lifeInsuredDetails)
                            .then(function(response) {
                                $scope.section.isCompleted = $scope.section.isCompleted && response.data.isPersonalDetailsValidForInforce;
                                $scope.section.isModelValid = $scope.section.isCompleted;
                                setWarnings();
                            })
                            .catch(function(response) {
                                processError(response.data, callBack);
                                throw response;
                            });
                    }

                    talPolicySectionsNotificationService.onPersonalDetailsChange({ personalDetails: $scope.ownerDetails });
                    talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section, []);
                    //talPolicyEditSectionsService.setSectionStatesOnSuccess($scope.section, response.data.warnings);

                    setWarnings();
                   
                    if (callBack) {
                        callBack();
                    }
                })
                .catch(function(response) {
                    processError(response.data, callBack);
                    throw response;
                });
        };

        talPolicySectionsNotificationService.registerOwnerTypeChangeEvent(function (ownerType) {
            $scope.ownerType = ownerType;
            updateSectionForOwner();

            if ($scope.ownerDetails) {
                $scope.ownerDetails.fundName = $scope.ownerType === 'SuperannuationFund' ? 'TASL / TAL Superfund' : '';
            }
        });
        
        function processError(modelState, callBack) {
            talFormModelStateService.updateModelState(modelState, $scope);
            $scope.section.isCompleted = false;
            $scope.section.isModelValid = false;
            talPolicyEditSectionsService.setSectionStatesOnError($scope.section);
            $scope.isDirty = true;

            if (callBack) {
                callBack();
            }
        }

        function setWarnings() {
            if ($scope.section.isModelValid === true) {
                talPolicyEditSectionsService.clearSectionState($scope.section);

                talPolicyEditSectionsService.addSectionState($scope.section, SECTION.STATUS.VALID);
            }
            else if ($scope.section.isModelValid === false) {
                talPolicyEditSectionsService.clearSectionState($scope.section);
                talPolicyEditSectionsService.addSectionState($scope.section, SECTION.STATUS.WARNING);
            }
            $scope.section.isModelValid = true;
        }

        function onDeactivation() {
            var deferred = $q.defer();
            talLoadingOverlayService.showFullPageOverlay();
            submitForm(function()
            {
                talLoadingOverlayService.hideFullPageOverlay();
                if($scope.isDirty){
                    deferred.reject();
                }else{
                    deferred.resolve();
                }
            });
            return deferred.promise;
        }

        function highlightCopiedFields(form) {
            form.highlight = true;
            $timeout(function () {
                form.highlight = false;
            }, 1000);
        }

        function copyRiskDetails() {
            $scope.lifeInsuredDetails.firstName = $scope.ownerDetails.firstName;
            $scope.lifeInsuredDetails.surname = $scope.ownerDetails.surname;
            $scope.lifeInsuredDetails.title = $scope.ownerDetails.title;
            $scope.lifeInsuredDetails.postcode = $scope.ownerDetails.postcode;
            $scope.lifeInsuredConsentsModel.expressConsent = $scope.ownerDetails.expressConsent;
            submitForm();
            highlightCopiedFields($scope.lifeInsuredDetails);
        }

        $scope.copyRiskDetails = copyRiskDetails;
        $scope.section.onActivationEvent = onActivation;
        $scope.section.onDeactivationEvent = onDeactivation;

        $scope.submitForm = submitForm;
    }

    module.controller('talRiskPersonalDetailsSectionController', talRiskPersonalDetailsSectionController );
    talRiskPersonalDetailsSectionController.$inject = ['$scope', 'FORM', 'talSalesPortalApiService', 'talFormModelStateService', 'talPolicySectionsNotificationService', 'talPolicyEditSectionsService', 'talLoadingOverlayService', '$q','SECTION', '$timeout'];

})(angular.module('salesPortalApp'));
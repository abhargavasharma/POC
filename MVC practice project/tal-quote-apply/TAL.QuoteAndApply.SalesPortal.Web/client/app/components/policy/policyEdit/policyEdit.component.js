(function (module) {
    'use strict';
    module.directive('talPolicyEdit', function () {
        return {
            templateUrl: '/client/app/components/policy/policyEdit/policyEdit.template.html',
            restrict: 'E',
            scope: {
                quoteNumber: '@',
                quoteEditSource: '@',
                externalRefRequired: '=',
                externalRefLabel: '@'
            },
            controller: 'talPolicyEditController'
        };
    });

    function talPolicyEditController($scope, talPolicyEditSectionsService, talPolicySectionsNotificationService, talSalesPortalApiService, $window, $uibModal, talLoadingOverlayService, FORM, SECTION) {
        var submissionErrorModalTemplate = '/client/app/components/policy/policyEdit/policySubmissionServiceFailure.modal.html';
        var submissionValidationModalTemplate = '/client/app/components/policy/policyEdit/policySubmissionValidationFailure.modal.html';

        function setStatusLabel(status){
            var statusLabel = {
                Incomplete: 'In progress',
                ReferredToUnderwriter: 'Referred to Underwriter',
                ReadyForInforce: 'Ready for Inforce',
                RaisedToPolicyAdminSystem: 'Submitted to PAS',
                Inforce: 'Inforce',
                FailedToSendToPolicyAdminSystem: 'Error: Failed to send to PAS',
                FailedDuringPolicyAdminSystemLoad: 'Error: Sent to PAS, failed during load.'
            };

            return statusLabel[status];
        }

        $scope.ownerTypes = [];
        
        talSalesPortalApiService.editPolicy($scope.quoteNumber, $scope.quoteEditSource)
            .then(function(response){
                $scope.vm = {
                    quoteReferenceNumber: response.data.quoteReferenceNumber,
                    risks: response.data.risks,
                    lastUpdated: response.data.lastSavedDate,
                    reviewSection: talPolicyEditSectionsService.reviewSection,
                    status: response.data.status,
                    statusLabel: setStatusLabel(response.data.status),
                    readOnly: response.data.readOnly,
                    userRole: response.data.userRole,
                    ownerType: response.data.ownerType,
                    externalRefRequired: $scope.externalRefRequired,
                    externalRefLabel: $scope.externalRefLabel
                };
                talLoadingOverlayService.hideFullPageOverlay();
                $scope.loadingPromise = talPolicyEditSectionsService.initialise($scope.vm.risks);
            })
            .catch(function(response){
                console.log(response);
            });

        talSalesPortalApiService.getAvailableOwnerTypes($scope.quoteNumber)
            .then(function(response) {
                $scope.ownerTypes = response.data;
            });

        $scope.isOwnerTypeSelectAvailable = function () {
            //if there is only 1 available owner type - hide the dropdown
            return $scope.ownerTypes.length > 1; 
        };

        $scope.isNoteSectionAvailable = function(){
            if($scope.vm) {
                return $scope.isAgentAccessing();
            }
            return false;
        };

        $scope.isUnderwriterAccessing = function(){
            if($scope.vm) {
                return $scope.vm.userRole === 'Underwriter';
            }
            return false;
        };
        $scope.isAgentAccessing = function(){
            if($scope.vm) {
                return $scope.vm.userRole === 'Agent';
            }
            return false;
        };
        $scope.isPolicyReadOnly = function(){
            if($scope.vm) {
                return $scope.vm.readOnly;
            }
            return false;
        };

        $scope.isPolicyInProgress = function(){
            if($scope.vm) {
                return $scope.vm.status === 'Incomplete';
            }
            return false;
        };
        $scope.isPolicyReferredToUnderwriter = function(){
            if($scope.vm) {
                return $scope.vm.status === 'ReferredToUnderwriter';
            }
            return false;
        };
        $scope.isPolicyReadyForInforce = function(){
            if($scope.vm) {
                return $scope.vm.status === 'ReadyForInforce';
            }
            return false;
        };
        $scope.isPolicyRaisedToPolicyAdminSystem = function(){
            if($scope.vm) {
                return $scope.vm.status === 'RaisedToPolicyAdminSystem' ||
                    $scope.vm.status === 'Inforce' ||
                    $scope.vm.status === 'FailedToSendToPolicyAdminSystem' ||
                    $scope.vm.status ==='FailedDuringPolicyAdminSystemLoad';
            }
            return false;
        };

        $scope.isFullAgentAccess = function(){
            return $scope.isPolicyInProgress() && $scope.isAgentAccessing() && !$scope.isPolicyReadOnly();
        };

        $scope.activateReview = function() {
            talPolicyEditSectionsService.activateSection($scope.vm.reviewSection);
        };

        $scope.ownerChanged = function () {
            talSalesPortalApiService.updatePolicyOwnerType($scope.quoteNumber, $scope.vm.ownerType)
                .then(function (response) {
                    
                    var risks = $scope.vm.risks[0];
                    var section =risks.sections.personalDetails;

                    var isCompleted = response.data.isCompleted;

                    $scope.vm.risks[0].sections.personalDetails.isCompleted = isCompleted;

                    if (isCompleted === true) {
                        talPolicyEditSectionsService.clearSectionState(section);

                        talPolicyEditSectionsService.addSectionState(section, SECTION.STATUS.VALID);
                    }
                    else if (isCompleted === false) {
                        talPolicyEditSectionsService.clearSectionState(section);
                        talPolicyEditSectionsService.addSectionState(section, SECTION.STATUS.WARNING);
                    }
                    $scope.vm.risks[0].sections.personalDetails.isModelValid = true;
                    talPolicySectionsNotificationService.onOwnerTypeChangeEvent($scope.vm.ownerType);
                })
                .catch(function (response) {
                    console.log(response);
                });
        };

        talPolicySectionsNotificationService.registerPersonalDetailsChangeEvent(function(personalDetails) {
            talPolicyEditSectionsService.applyPersonalDetailsChangeToSelectedRisk(personalDetails);
        });

        //check if client name is valid in Customer Details form
        $scope.validateClientName = function (firstname, surname) {
            return (firstname || surname);
        };

        var launchModal = function(templateUrl, validations, reload){
            var modal = $uibModal.open({
                animation: true,
                templateUrl: templateUrl,
                controller: 'policySubmissionModalController',
                size: 'md',
                resolve: {
                    validationErrors : function() {
                        return { risksWithIssues: validations } ;
                    }
                }
            });

            if(reload) {
                modal.result.then(function() {$window.location.reload(); }, function() {$window.location.reload(); });
            }
        };

        var buildValidationErrorsFromResponse = function(response){
            var risksWithIssues = [];

            _.each(response.riskValidations, function (riskVal) {

                var sectionsWithIssues = [];

                _.each(riskVal.sectionsValidations, function (section) {
                   
                    if(!section.completed) {
                        sectionsWithIssues.push(section.sectionName);
                    }
                });

                if(sectionsWithIssues.length > 0){
                    var vmRisk = _.find($scope.vm.risks, function(r) {
                        return r.riskId === riskVal.riskId;
                    });

                    var risk = {
                        firstName: vmRisk.firstName,
                        surname: vmRisk.surname,
                        sectionsWithIssues: sectionsWithIssues
                    };

                    risksWithIssues.push(risk);
                }
            });

            return risksWithIssues;
        };

        $scope.validateSectionsAndSubmit = function () {

            talLoadingOverlayService.showFullPageOverlay();

            talSalesPortalApiService.submitPolicy($scope.quoteNumber)
                .then(function(response) {

                    if(response.data.successful){
                        $window.location.reload();
                    }
                    else {
                        
                        talLoadingOverlayService.hideFullPageOverlay();
                        if(response.data.serverError) {
                            launchModal(submissionErrorModalTemplate, null, true);
                        }
                        else{
                            launchModal(submissionValidationModalTemplate, buildValidationErrorsFromResponse(response.data));
                        }
                    }

                })
                .catch(function () {
                    talLoadingOverlayService.hideFullPageOverlay();
                    launchModal(submissionErrorModalTemplate, null, true);
                });
        };
    }

    module.controller('talPolicyEditController', talPolicyEditController);
    talPolicyEditController.$inject = ['$scope', 'talPolicyEditSectionsService', 'talPolicySectionsNotificationService', 'talSalesPortalApiService', '$window', '$uibModal', 'talLoadingOverlayService', 'FORM', 'SECTION'];

})(angular.module('salesPortalApp'));

'use strict';

describe('Service: talPolicyEditSectionsService', function () {

    // load the service's module
    beforeEach(module('salesPortalApp'));

    // instantiate service
    var talPolicyEditSectionsService, $timeout;
    beforeEach(inject(function (_talPolicyEditSectionsService_, _$timeout_) {
        talPolicyEditSectionsService = _talPolicyEditSectionsService_;
        $timeout = _$timeout_;
    }));

    it('should do something', function () {
        expect(!!talPolicyEditSectionsService).toBe(true);
    });

    var addActivationEvents = function(risks){
        _.each(risks, function (risk) {
            _.each(risk.sections, function (section) {
                section.onActivationEvent = function() {};
            });
        });
    };

    describe('initialise', function(){
        it('each risk should get section assigned to them', function () {
            var risks = [{isOwner: true}, {isOwner: false}];
            talPolicyEditSectionsService.initialise(risks);

            expect(risks[0].sections).not.toBe(null);
            expect(risks[1].sections).not.toBe(null);
        });

        it('owner personal details section should be assigned as open', function () {
            var risks = [{isOwner: true}, {isOwner: false}];
            talPolicyEditSectionsService.initialise(risks);
            addActivationEvents(risks);
            $timeout.flush();

            expect(risks[0].sections.personalDetails.open).toBe(true);
        });
    });

    describe('activateSection', function() {
        it('if current section is not valid, section not opened', function () {

            var risks = [{isOwner: true}, {isOwner: false}];
            talPolicyEditSectionsService.initialise(risks);
            addActivationEvents(risks);
            $timeout.flush();

            risks[0].sections.personalDetails.isModelValid = false;
            talPolicyEditSectionsService.activateSection(risks[0].sections.insurancePlans);

            expect(risks[0].sections.personalDetails.open).toBe(true);
            expect(risks[0].sections.insurancePlans.open).toBe(false);
        });

        it('if current section is valid, old section closed and new section opened', function () {

            var risks = [{isOwner: true}, {isOwner: false}];
            talPolicyEditSectionsService.initialise(risks);
            addActivationEvents(risks);
            $timeout.flush();

            risks[0].sections.personalDetails.isModelValid = true;
            talPolicyEditSectionsService.activateSection(risks[0].sections.insurancePlans);

            expect(risks[0].sections.personalDetails.open).toBe(false);
            expect(risks[0].sections.insurancePlans.open).toBe(true);
        });

        it('section to activate event called', function () {

            var risks = [{isOwner: true}, {isOwner: false}];
            talPolicyEditSectionsService.initialise(risks);
            addActivationEvents(risks);
            $timeout.flush();

            risks[0].sections.insurancePlans.onActivationEvent = function(){};
            spyOn(risks[0].sections.insurancePlans, 'onActivationEvent');

            risks[0].sections.personalDetails.isModelValid = true;
            talPolicyEditSectionsService.activateSection(risks[0].sections.insurancePlans);

            expect(risks[0].sections.insurancePlans.onActivationEvent).toHaveBeenCalled();
        });

    });
});

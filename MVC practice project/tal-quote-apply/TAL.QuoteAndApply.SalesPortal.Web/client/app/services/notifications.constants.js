(function (module) {
    'use strict';

    module.constant('NOTIFICATIONS', {
        UNDERWRITING: {
            QUESTION_ANSWERED: 'QuestionAnswered',
            INTERVIEW_LOADED: 'InterviewReady',
            INTERVIEW_PRECONDITION_ERROR: 'InterviewPreconditionError',
            OUTCOME_OVERRIDEN: 'OutcomeOverriden',
            LOADINGS_OVERRIDEN: 'LoadingsOverriden',
            EXCLUSIONS_OVERRIDEN: 'ExclusionsOverriden',
            NOTE_ADDED: 'NoteAdded'
        }
    });

})(angular.module('salesPortalApp'));


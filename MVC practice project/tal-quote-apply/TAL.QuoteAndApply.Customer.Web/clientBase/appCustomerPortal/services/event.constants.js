(function (module) {
    'use strict';

    module.constant('EVENT', {
        RISK_ID:{
            UPDATE: 'risk-id:update'
        },
        CHAT: {
            CALLBACK: 'chat:request-callback',
            CHAT_AVAILABILITY: 'chat:chat-availability',
            CHAT_TO_AGENT: 'chat:chat-to-agent',
            CHECK_AVAILABILITY: 'chat:check-availability'
        },
        SAVE: {
            TRIGGER_FORCED_SAVE: 'save:trigger-forced-save',
            HIDE_MOBILE_FOOTER: 'save:hide-mobile-footer'
        },
        QUOTE: {
            QUOTE_REF_NUMBER: 'quote:quote-reference-number',
            SHOW_QUOTE_REF_NUMBER: 'quote:show-quote-reference-number'
        },
        SUBMIT: {
            ON_ERROR: 'form:error'
        },
        PURCHASE: {
            CONTACT_DETAILS: 'purchase:trigger-contact-change'
        }
    });

})(angular.module('appCustomerPortal'));


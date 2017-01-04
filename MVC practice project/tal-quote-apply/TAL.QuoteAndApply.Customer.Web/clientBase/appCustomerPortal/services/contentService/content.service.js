'use strict';

angular.module('appCustomerPortal').service('talContentService', function ($interpolate, $templateCache) {

    //Use attributes in sharedContent as variables in content text by enclosing in double curlies
    var sharedContent = {
        contact: {
            talPhone: '131 825',
            unsubscribePhone: '1800 005 962',
            confirmationPhone:'1300 209 088',
            openingHours: 'Mon-Fri 9.00am-7.00pm'
        },
        buttons: {
            review: {
                accept: 'Purchase Cover',
                refer: 'Submit application'
            }
        },
        links: {
            pds: 'https://www.tal.com.au/-/media/869a7a8cba6742f29ab24c9caee1ccc2.pdf',
            fsg: 'https://www.tal.com.au/-/media/3774f25129ce4124a5948eb3f7d726f5.pdf',
            combinedPdsFsg: 'https://www.tal.com.au/lifetimeprotection-pds',
            privacyPolicy: 'https://www.tal.com.au/privacy-policy',
            generalAdvice: 'https://www.tal.com.au/ways-to-get-insurance/find-an-adviser',
            security: 'https://www.tal.com.au/security'
        },
        consent:{
            dncSelection: 'Please don\'t send me any marketing material.',
            termsAgree: 'I have read and agree to the terms above.'
        }
    };

    var paymentFrequencyPer = {
        'Monthly': 'month',
        'Quarterly': 'quarter',
        'HalfYearly': 'half year',
        'Half Yearly': 'half year',
        'Yearly': 'year'
    };

    var coverBlockContent = {
        'DTHAC': {
            shortTitle: 'Accident Cover',
            fullTitle: 'Accident Cover',
            selectCoverOptionsDescription: '<p>An Accident Cover payment is made in a lump sum if you die as a result of an accident, such as a car crash or accidental drowning.</p>',
            description: '<p>An Accident Cover payment is made in a lump sum if you die as a result of an accident, such as a car crash or accidental drowning.</p>',
            includeText: 'Include Accident Cover',
            excludeText: 'Exclude Accident Cover'
        },
        'DTHIC': {
            shortTitle: 'Illness Cover',
            fullTitle: 'Illness Cover',
            selectCoverOptionsDescription: '<p>An Illness Cover payment is made in a lump sum if you die or are diagnosed as terminally ill as a result of an illness such as cancer, stroke or dementia.</p>',
            description: '<p>An Illness Cover payment is made in a lump sum if you die or are diagnosed as terminally ill as a result of an illness, such as cancer, stroke or dementia.</p>',
            includeText: 'Include Illness Cover',
            excludeText: 'Exclude Illness Cover',
            coverBenefits: ['Terminal Illness Cover']
        },
        'DTHASC': {
            shortTitle: 'Adventure Sports Cover',
            fullTitle: 'Adventure Sports Cover',
            selectCoverOptionsDescription: '<p>An Adventure Sports Cover payment is made in a lump sum if you die as a result of participating in a high-risk sport that usually involves speed, height, high levels of physical exertion, or highly specialised gear such as SCUBA diving, motor racing, rock climbing or any other activity.</p>' +
                                            '<p>In order to have Adventure Sports Cover you must also have Accident Cover.</p>',
            description: '<p>Sports Cover payments are made if you are left totally disabled or partially disabled and can\'t work as a result of a sports injury such as dislocating your knee. </br></br>In order to have Sports Cover you must also have Accident Cover.</p>',
            includeText: 'Include Adventure Sports Cover',
            excludeText: 'Exclude Adventure Sports Cover'
        },
        'TPDDTHAC': {
            shortTitle: 'TPD: Accident',
            fullTitle: 'Accident Cover',
            description: '<p>TPD: Accident yo, commodo lacus at, sollicitudin est. Cras nulla mi, lobortis vel ligula ac, cursus ullamcorper augue. Fusce venenatis iaculis consectetur. Vivamus venenatis massa et tellus molestie pretium. Fusce convallis enim ac nisl venenatis tempor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Quisque eu vehicula nisi. Praesent maximus cursus felis, a iaculis enim molestie nec. Mauris congue justo ut mi pretium fringilla. Nulla vel tempus diam. Etiam a velit ac turpis aliquet placerat. Integer eu ligula dui. In bibendum, massa non pellentesque consequat, metus arcu egestas dolor, at porttitor justo dui porta arcu. Aenean neque enim, cursus sit amet elit et, volutpat tincidunt eros. Integer sagittis ligula ut odio pretium vehicula.</p>',
            includeText: 'Include Accident Cover',
            excludeText: 'Dont include Accident Cover'
        },
        'TPDDTHIC': {
            shortTitle: 'TPD: Illness',
            fullTitle: 'Illness Cover',
            description: '<p>TPD: Illness yo, commodo lacus at, sollicitudin est. Cras nulla mi, lobortis vel ligula ac, cursus ullamcorper augue. Fusce venenatis iaculis consectetur. Vivamus venenatis massa et tellus molestie pretium. Fusce convallis enim ac nisl venenatis tempor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Quisque eu vehicula nisi. Praesent maximus cursus felis, a iaculis enim molestie nec. Mauris congue justo ut mi pretium fringilla. Nulla vel tempus diam. Etiam a velit ac turpis aliquet placerat. Integer eu ligula dui. In bibendum, massa non pellentesque consequat, metus arcu egestas dolor, at porttitor justo dui porta arcu. Aenean neque enim, cursus sit amet elit et, volutpat tincidunt eros. Integer sagittis ligula ut odio pretium vehicula.</p>',
            includeText: 'Include Illness Cover',
            excludeText: 'Dont include Illness Cover'
        },
        'TPDDTHASC': {
            shortTitle: 'TPD: Adventure Sports',
            fullTitle: 'Adventure Sports Cover',
            description: '<p>TPD: Adventure Sports yo, commodo lacus at, sollicitudin est. Cras nulla mi, lobortis vel ligula ac, cursus ullamcorper augue. Fusce venenatis iaculis consectetur. Vivamus venenatis massa et tellus molestie pretium. Fusce convallis enim ac nisl venenatis tempor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Quisque eu vehicula nisi. Praesent maximus cursus felis, a iaculis enim molestie nec. Mauris congue justo ut mi pretium fringilla. Nulla vel tempus diam. Etiam a velit ac turpis aliquet placerat. Integer eu ligula dui. In bibendum, massa non pellentesque consequat, metus arcu egestas dolor, at porttitor justo dui porta arcu. Aenean neque enim, cursus sit amet elit et, volutpat tincidunt eros. Integer sagittis ligula ut odio pretium vehicula.</p>',
            includeText: 'Include Adventure Sports Cover',
            excludeText: 'Dont include Adventure Sports Cover'
        },
        'TPSAC': {
            shortTitle: 'Accident Cover',
            fullTitle: 'Accident Cover',
            selectCoverOptionsDescription: '<p>An Accident Cover payment is made in a lump sum if you become totally and permanently disabled as a result of injuries sustained in an accident, such as a car crash.</p>',
            description: '<p>An Accident Cover payment is made in a lump sum if you become totally and permanently disabled as a result of injuries sustained in an accident such as a car crash.</p>',
            includeText: 'Include Accident Cover',
            excludeText: 'Exclude Accident Cover'
        },
        'TPSIC': {
            shortTitle: 'Illness Cover',
            fullTitle: 'Illness Cover',
            selectCoverOptionsDescription: '<p>An Illness Cover payment is made in a lump sum if you become totally and permanently disabled as a result of illnesses such as cancer, heart disease or organ failure.</p>',
            description: '<p>An Illness Cover payment is made in a lump sum if you become totally and permanently disabled as a result of illnesses such as cancer, heart disease or organ failure.</p>',
            includeText: 'Include Illness Cover',
            excludeText: 'Exclude Illness Cover'
        },
        'TPSASC': {
            shortTitle: 'Sports Cover',
            fullTitle: 'Sports Cover',
            selectCoverOptionsDescription: '<p>A Sports Cover payment is made in a lump sum if you become totally and permanently disabled as a result of participating in a sporting activity, such as football or rock climbing. </br></br> In order to have Sports Cover you must also have Accident Cover.</p>',
            description: '<p>A Sports Cover payment is made in a lump sum if you become totally and permanently disabled as a result of participating in a sporting activity, such as football or rock climbing. </br></br> In order to have Sports Cover you must also have Accident Cover.</p>',
            includeText: 'Include Sports Cover',
            excludeText: 'Exclude Sports Cover'
        },
        'TRADTHSIN': {
            shortTitle: 'CI: Serious Injury',
            fullTitle: 'Serious Injury Cover',
            description: '<p>CI: Serious Injury yo, commodo lacus at, sollicitudin est. Cras nulla mi, lobortis vel ligula ac, cursus ullamcorper augue. Fusce venenatis iaculis consectetur. Vivamus venenatis massa et tellus molestie pretium. Fusce convallis enim ac nisl venenatis tempor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Quisque eu vehicula nisi. Praesent maximus cursus felis, a iaculis enim molestie nec. Mauris congue justo ut mi pretium fringilla. Nulla vel tempus diam. Etiam a velit ac turpis aliquet placerat. Integer eu ligula dui. In bibendum, massa non pellentesque consequat, metus arcu egestas dolor, at porttitor justo dui porta arcu. Aenean neque enim, cursus sit amet elit et, volutpat tincidunt eros. Integer sagittis ligula ut odio pretium vehicula.</p>',
            includeText: 'Include Serious Injury Cover',
            excludeText: 'Dont include Serious Injury Cover'
        },
        'TRADTHCC': {
            shortTitle: 'CI: Cancer',
            fullTitle: 'Cancer Cover',
            description: '<p>CI: Cancer yo, commodo lacus at, sollicitudin est. Cras nulla mi, lobortis vel ligula ac, cursus ullamcorper augue. Fusce venenatis iaculis consectetur. Vivamus venenatis massa et tellus molestie pretium. Fusce convallis enim ac nisl venenatis tempor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Quisque eu vehicula nisi. Praesent maximus cursus felis, a iaculis enim molestie nec. Mauris congue justo ut mi pretium fringilla. Nulla vel tempus diam. Etiam a velit ac turpis aliquet placerat. Integer eu ligula dui. In bibendum, massa non pellentesque consequat, metus arcu egestas dolor, at porttitor justo dui porta arcu. Aenean neque enim, cursus sit amet elit et, volutpat tincidunt eros. Integer sagittis ligula ut odio pretium vehicula.</p>',
            includeText: 'Include Cancer Cover',
            excludeText: 'Dont include Cancer Cover'
        },
        'TRADTHSIC': {
            shortTitle: 'CI: Medical Trauma',
            fullTitle: 'Medical Trauma Cover',
            description: '<p>CI: Medical Trauma yo, commodo lacus at, sollicitudin est. Cras nulla mi, lobortis vel ligula ac, cursus ullamcorper augue. Fusce venenatis iaculis consectetur. Vivamus venenatis massa et tellus molestie pretium. Fusce convallis enim ac nisl venenatis tempor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Quisque eu vehicula nisi. Praesent maximus cursus felis, a iaculis enim molestie nec. Mauris congue justo ut mi pretium fringilla. Nulla vel tempus diam. Etiam a velit ac turpis aliquet placerat. Integer eu ligula dui. In bibendum, massa non pellentesque consequat, metus arcu egestas dolor, at porttitor justo dui porta arcu. Aenean neque enim, cursus sit amet elit et, volutpat tincidunt eros. Integer sagittis ligula ut odio pretium vehicula.</p>',
            includeText: 'Include Medical Trauma Cover',
            excludeText: 'Dont include Medical Trauma Cover',
            coverBenefits: ['Advancement Benefit']
        },
        'TRSSIC': {
            shortTitle: 'Critical Illness Cover',
            fullTitle: 'Critical Illness Cover',
            selectCoverOptionsDescription: '<p>A Critical Illness Cover payment is made in a lump sum if you are diagnosed with an insured condition, such as a heart attack, dementia, a stroke, loss of hearing or chronic kidney failure. </br></br> For a full list of insured events, please refer to Critical Illness in the glossary within the PDS.</p>',
            description: '<p>A Critical Illness Cover payment is made in a lump sum if you are diagnosed with an insured event such as a heart attack, dementia, a stroke, loss of hearing, or chronic kidney failure. </br></br>For a full list of insured events, please refer to Critical Illness in the glossary within the PDS.</p>',
            includeText: 'Include Critical Illness Cover',
            excludeText: 'Exclude Critical Illness Cover'
        },
        'TRSCC': {
            shortTitle: 'Cancer Cover',
            fullTitle: 'Cancer Cover',
            selectCoverOptionsDescription: '<p>A Cancer Cover payment is made in a lump sum if you are diagnosed with an insured form of cancer such as malignant tumours, benign brain tumours or specified chronic Leukaemias. </br></br> For a full list of insured events, please refer to Cancer in the glossary within the PDS.</p>',
            description: '<p>A Cancer Cover payment is made in a lump sum if you are diagnosed with an insured form of cancer such as malignant tumours, benign brain tumours or specified chronic Leukaemias. </br></br>For a full list of insured events please refer to Cancer in the glossary within the PDS.</p>',
            includeText: 'Include Cancer Cover',
            excludeText: 'Exclude Cancer Cover',
            coverBenefits: ['Cancer Advancement Benefit']
        },
        'TRSSIN': {
            shortTitle: 'Critical Injury Cover',
            fullTitle: 'Critical Injury Cover',
            selectCoverOptionsDescription: '<p>A Critical Injury Cover payment is made in a lump sum if you are suffering from an insured event due to an accident such as severe burns, or an injury that resulted in you being admitted to intensive care in hospital. </br></br> For a full list of insured events, please refer to Critical Injury in the glossary within the PDS.</p>',
            description: '<p>A Critical Injury Cover payment is made in a lump sum if you are suffering from an insured event due to an accident such as severe burns, or an injury that resulted in you being admitted to intensive care in hospital. </br></br>For a full list of insured events, please refer to Critical Injury in the glossary within the PDS.</p>',
            includeText: 'Include Critical Injury Cover',
            excludeText: 'Exclude Critical Injury Cover'
        },
        'IPSAC': {
            shortTitle: 'Accident Cover',
            fullTitle: 'Accident Cover',
            selectCoverOptionsDescription: '<p>Accident Cover payments are made if you are left totally disabled or partially disabled and can\'t work after an accident, such as breaking a leg.</p>',
            description: '<p>Accident Cover payments are made if you can\'t work and are left totally disabled or partially disabled after an accident such as breaking a leg.</p>',
            includeText: 'Include Accident Cover',
            excludeText: 'Exclude Accident Cover'
        },
        'IPSIC': {
            shortTitle: 'Illness Cover',
            fullTitle: 'Illness Cover',
            selectCoverOptionsDescription: '<p>Illness Cover payments are made if you are left totally disabled or partially disabled and can\'t work due to an illness, such as pneumonia. </p>',
            description: '<p>Illness Cover payments are made if you can\'t work and are left totally disabled or partially disabled due to an illness such as pneumonia.</p>',
            includeText: 'Include Illness Cover',
            excludeText: 'Exclude Illness Cover'
        },
        'IPSSC': {
            shortTitle: 'Sports Cover',
            fullTitle: 'Sports Cover',
            selectCoverOptionsDescription: '<p>Sports Cover payments are made if you are left totally disabled or partially disabled and can\'t work as a result of a sports injury, such as a ligament injury.  </br></br> In order to have Sports Cover you must also have Accident Cover.</p>',
            description: '<p>Sports Cover payments are made if you can\'t work and are left totally disabled or partially disabled as a result of a sports injury such as dislocating your knee. </br></br>In order to have Sports Cover you must also have Accident Cover.</p>',
            includeText: 'Include Sports Cover',
            excludeText: 'Exclude Sports Cover'
        }
    };

    var planBlockContent = {
        'DTH': {
            planTitle: 'Life <br/>Insurance',
            planTitleRaw: 'Life Insurance', //For when you don't want HTML in the title
            coversButton: 'Life Insurance Options',
            learnMore: 'Learn more <br/>about Life Insurance',
            learnMoreModalTitle: 'Learn more about Life Insurance',
            learnMoreModalContent: '<p>Life Insurance can pay up to $1.5 million in a lump sum if you die, or if you\'re diagnosed with a terminal illness.</p>' +
                                   '<p><strong>An example of how Life Insurance could work</strong></p>' +
                                   '<p>Mark, a 55 year old builder, is diagnosed with heart disease after experiencing chest pains, and given 6 months to live.</p>' +
                                   '<p>Under his Life Insurance policy he receives a $1,323,000* payment. With it he pays off his mortgage, adds to his wife\'s super fund and takes a trip overseas to visit his parents.</p>' +
                                   '<p>Mark also pays all his medical bills to make sure his wife is left with no debts when he passes away.</p>' +
                                   '<p><a href="{{links.combinedPdsFsg}}" target="_blank">Download the PDS</a></p>' +
                                   '<p>*<small>The above example is illustrative only of how Life Insurance can work. For more information on TAL Lifetime Protection, please see the PDS or call&nbsp;<a href="tel:{{ contact.talPhone }}" class="tal-link--strong">{{ contact.talPhone }} </a>&nbsp;</small></p>',
            inEligibleReason: 'to find out more about your Life Insurance quote. To help us quickly identify your quote, we\'ll ask you for your quote\'s reference number, which is located top right of screen.',
            coverAmounts: [500000, 1000000, 1500000],
            paymentFrequencyText: '',
            coverAmountLabel: 'Life Insurance amount',
            whenOffDescription: '<p><strong>Want to include Life Insurance?</strong></p><p>Life insurance protects your family\'s future and gives them options if you are no longer around.</p>',
            whenOffDescriptionForSelectCover: 'Life insurance protects your family\'s future and gives them options if you are no longer around.',
            reviewDescription: 'Your beneficiaries will receive a <strong>{{ctrl.plan.selectedCoverAmount | wholeDollars}}</strong> lump sum payment if you die.',
            reviewDescriptionExtra: '',
            exclusions: [
                'Suicide is not covered in the first 13 months.',
                'Visiting a country that has an Australian Department of Foreign Affairs and Trade (DFAT) \'Do not travel\' warning, in force during your visit.'],
            benefits: [
                'Death Benefit',
                'Funeral Advancement Benefit (payable once a death certificate has been provided)',
                'Inflation Protection',
                'Future Life Events',
                'Cover pause'],
            benefitHeading: 'Life Insurance',
            exclusionHeading:'What you\'re not covered for in the event you die',
            exclusionDeclaration: 'There may be additional exclusions applied to your Life Insurance depending on the cover options you have chosen. Please refer to the PDS for full details.'
        },
        'TRS': {
            planTitle: 'Recovery Insurance',
            coversButton: 'Recovery Insurance Options',
            learnMore: 'Learn more <br/>about Recovery Insurance',
            learnMoreModalTitle: 'Learn more about Recovery Insurance',
            learnMoreModalContent: '<p>Recovery Insurance can pay up to $500,000 in a lump sum if you suffer a serious injury or illness such as cancer, stroke or severe burns.</p>' +
                                   '<p><strong>An example of how Recovery Insurance could work</strong></p>' +
                                   '<p>Sophie, a 52 year old bookkeeper, is diagnosed with Thyroid Cancer after her GP runs tests for pain and swelling in her neck.</p>' +
                                   '<p>Sophie receives $381,385* through her Recovery Insurance. She uses the payment to clear her mortgage and pay for a holiday to help her recuperate before returning to work.</p>' +
                                   '<p><a href="{{links.combinedPdsFsg}}" target="_blank">Download the PDS</a></p>' +
                                   '<p>*<small>The above example is illustrative only of how Recovery Insurance can work. For more information on TAL Lifetime Protection, please see the PDS or call&nbsp;<a href="tel:{{ contact.talPhone }}" class="tal-link--strong">{{ contact.talPhone }} </a>&nbsp;</small></p>',
            inEligibleReason: 'to find out more about your Recovery Insurance quote. To help us quickly identify your quote, we\'ll ask you for your quote\'s reference number, which is located top right of screen.',
            coverAmounts: [500000, 1000000, 1500000],
            paymentFrequencyText: '',
            coverAmountLabel: 'Recovery Insurance amount',
            whenOffDescription: '<p><strong>Want to include Recovery Insurance?</strong></p>' +
                                '<p>Recovery Insurance gives you choices so you can still make the most of life if you have a serious injury or illness.</p>',
            whenOffDescriptionForSelectCover: 'Recovery Insurance gives you choices so you can still make the most of life if you have a serious injury or illness.',
            reviewDescription: 'You will receive a <strong>{{ctrl.plan.selectedCoverAmount | wholeDollars}}</strong> lump sum payment if you suffer an insured event.',
            reviewDescriptionExtra: '',
            attachRiderDescription: '<p>Recovery Insurance can be attached to a Life Insurance policy or purchased as stand-alone policy.</p><ul class="tal-strong-list"><li>Making Critical Illness Cover bundled will be cheaper but if you make a claim the payout amount will reduce your Life Cover</li><li>Holding Critical Illness Cover as a stand-alone policy is more expensive but if you make a claim your Life Cover will be unaffected</li></ul><h3 class="h5">Example of a claim: Victor</h3><p>Victor, a 42 year old Software Developer, was paralysed in a car accident. His Life Cover of $1,750,000 with us included $500,000 TPD cover as a bundle. Victor received the $500,000 payout and used it to fund ongoing physical therapy and to pay off his mortgage. He now has $1,250,000 remaining on his Life Cover.</p>',
            attachRiderDescriptionHtml: '/client/appCustomerPortal/services/contentService/attachRiderCI.template.html',
            exclusions: [
                'You\'re not covered for any event where you do not survive for at least 14 days.',
                'Suicide is not covered in the first 13 months.'],
            benefits: [
                'Inflation Protection',
                'Future Life Events',
                'Cover Pause'],
            benefitHeading: 'Recovery Insurance',
            exclusionHeading: 'What you are not covered for',
            exclusionDeclaration: 'Some Critical Illness and Cancer Cover events will not be covered if they occur or are diagnosed within three months of new cover. Please refer to the PDS for full details. </br></br>There may be additional exclusions applied to your Recovery Insurance depending on the cover options you have chosen. Please refer to the PDS for full details.'
        },
        'TRADTH': {
            planTitle: 'Recovery Insurance',
            coversButton: 'Recovery Insurance Options',
            learnMore: 'Learn more <br/>about Recovery Insurance',
            coverAmounts: [500000, 1000000, 1500000],
            reviewText: 'If you sustain a serious injury or you are diagnosed with a specified critical illness such as cancer or motor neurone disease you will receive a tax-free lump sum of <strong>[CI Rider Cover Amount]</strong> (leaving you with <strong>[Leftover Cover Amount]</strong> of Life Cover).',
            coverBenefits: ['Advancement Benefit']
        },
        'TPS': {
            planTitle: 'Total Permanent Disability Insurance (TPD)',
            coversButton: 'TPD Insurance Options',
            learnMore: 'Learn more <br/>about TPD Insurance',
            learnMoreModalTitle: 'Learn more about Total Permanent Disability Insurance  (TPD)',
            learnMoreModalContent: '<p>TPD Insurance can pay up to $1.5 million in a lump sum if you suffer a total and permanent disability that prevents you working again.</p>' +
                                   '<p><strong>An example of how TPD Insurance could work</strong></p>' +
                                   '<p>Russell, a 34 year old executive, is left totally and permanently disabled as a result of a stroke at work. Through his TPD Insurance he receives $593,296* which he uses part of to pay off his mortgage.</p>' +
                                   '<p>He also invests a portion of his payment, and the returns provide an income that covers his ongoing rehabilitation.</p>' +
                                   '<p><a href="{{links.combinedPdsFsg}}" target="_blank">Download the PDS</a></p>' +
                                   '<p>*<small>The above example is illustrative only of how TPD can work. For more information on TAL Lifetime Protection, please see the PDS or call&nbsp;<a href="tel:{{ contact.talPhone }}" class="tal-link--strong">{{ contact.talPhone }} </a></small></p>',
            inEligibleReason: 'to find out more about your TPD quote. To help us quickly identify your quote, we\'ll ask you for your quote\'s reference number, which is located top right of screen.',
            coverAmounts: [500000, 1000000, 1500000],
            paymentFrequencyText: '',
            coverAmountLabel: 'TPD Insurance amount',
            whenOffDescription: '<p><strong>Want to include TPD?</strong></p>' +
                                '<p>TPD gives you options to help you live a better quality of life if you are totally and permanently disabled and can\'t work.</p>',
            whenOffDescriptionForSelectCover: 'TPD gives you options to help you live a better quality of life if you are totally and permanently disabled and can\'t work.',
            reviewDescription: 'You will receive a <strong>{{ctrl.plan.selectedCoverAmount | wholeDollars}}</strong> lump sum payment if you suffer a total and  permanent disability that either prevents you ever working again, or results in you being unable to perform some activities of daily life.',
            reviewDescriptionExtra: '',
            attachRiderDescription: '<p>TPD Insurance can be attached to a Life Insurance policy or purchased as stand-alone policy.</p><ul class="tal-strong-list"><li>Making TPD Cover bundled will be cheaper but if you make a claim the payout amount will reduce your Life Cover</li><li>Holding TPD Cover as a stand-alone policy is more expensive but if you make a claim your Life Cover will be unaffected</li></ul><h3 class="h5">Example of a claim: Victor</h3><p>Victor, a 42 year old Software Developer, was paralysed in a car accident. His Life Cover of $1,750,000 with us included $500,000 TPD cover as a bundle. Victor received the $500,000 payout and used it to fund ongoing physical therapy and to pay off his mortgage. He now has $1,250,000 remaining on his Life Cover.</p>',
            attachRiderDescriptionHtml: '/client/appCustomerPortal/services/contentService/attachRiderTPD.template.html',
            exclusions: [
                'You\'re not covered for any event where you do not survive for at least 14 days.',
                'Suicide is not covered in the first 13 months.'],
            benefits: [
                'Total Permanent Disability Benefit',
                'Inflation Protection',
                'Future Life Events',
                'Cover Pause'],
            benefitHeading: 'TPD Insurance',
            exclusionHeading: 'What you\'re not covered for in the event you become totally and permanently disabled ',
            exclusionDeclaration: 'There may be additional exclusions applied to your TPD Insurance depending on the cover options you have chosen. Please refer to the PDS for full details.'
        },
        'TPDDTH': {
            planTitle: 'Total Permanent <br/>Disability Insurance',
            coversButton: 'TPD Insurance Options',
            learnMore: 'Learn more <br/>about (TPD) Insurance',
            coverAmounts: [500000, 1000000, 1500000],
            reviewText: 'In the event that you\'re unable to work in any occupation due to an accident or illness you will receive a tax-free lump sum of <strong>[TPD Rider Cover Amount]</strong> (leaving you with <strong>[Leftover Cover Amount]</strong> of Life cover).'
        },
        'IP': {
            planTitle: 'Income Protection',
            coversButton: 'Income Protection Options',
            learnMore: 'Learn more about Income Protection',
            learnMoreModalTitle: 'Learn more about Income Protection',
            learnMoreModalContent: '<p>Income Protection can pay up to 75% of your regular income, up to $10,000 per month for up to 5 years, if you can\'t work due to an illness or injury.</p>' +
                                  '<p><strong>An example of how Income Protection could work</strong></p>' +
                                  '<p>Sushant is 41 when he is injured from a fall and can\'t work for a number of months. Sushant\'s Income Protection Insurance pays 75% of his pre-accident income. </p>' +
                                  '<p>One month after the end of his cover\'s  4 week waiting period, Sushant\'s receives his first monthly payment , which helps to cover the cost of groceries, mortgage repayments, electricity and the all\-important internet bill.*</p>' +
                                  '<p><a href="{{links.combinedPdsFsg}}" target="_blank">Download the PDS</a></p>' +
                                  '<p>*<small>The above example is illustrative only of how Income Protection can work. For more information on TAL Lifetime Protection, please see the PDS or call&nbsp;<a href="tel:{{ contact.talPhone }}" class="tal-link--strong">{{ contact.talPhone }} </a>&nbsp;</small></p>',
            inEligibleReason: 'to find out more about your Income Protection quote. To help us quickly identify your quote, we\'ll ask you for your quote\'s reference number, which is located top right of screen.',
            coverAmounts: [1500, 3000, 4500, 6000],
            paymentFrequencyText: 'per month',
            coverAmountLabel: 'Income Protection amount',
            whenOffDescription: '<p><strong>Want to include Income Protection?</strong></p>' +
                                '<p>Income Protection provides you with an income to help you continue living your life if you have had an injury or illness and can\'t work.</p>',
            whenOffDescriptionForSelectCover: 'Income Protection provides you with an income to help you continue living your life if you have had an injury or illness and can\'t work.',
            reviewDescription: 'You\'ll receive  <strong>{{ctrl.plan.selectedCoverAmount | wholeDollars}} per month for {{ctrl.getVariableSelectedText(\'benefitPeriod\')}}</strong> (or until you return to work) in the event of an illness or injury that prevents you from working.',
            reviewDescriptionExtra: '<p>Feel free to change your Waiting Period (how many weeks you\'ll wait before your payments start accruing). </p>' +
                                  '<p>You can also change your Income Protection benefit period (the maximum period you want to receive payments while you are injured or sick) and fine tune other elements below.</p>',
            exclusions: [
                'an intentional, self-inflicted act',
                'pregnancy, giving birth, miscarrying or termination of a pregnancy',
                'war, terrorism, civil commotion or unrest, guerrilla or insurgent activities in countries outside Australia',
                'visiting a country that has an Australian Department of Foreign Affairs and Trade (DFAT) \'Do not travel\' warning, in force during your visit',
                'any illness, injury or sports injury that occurs as a result of, or during the course of , committing or attempting to commit  a criminal offence',
                'mental health related illnesses, or any mental health related illnesses as a result of an accidental injury, sports injury or illness after a benefit has been paid for two years.'],
            benefits: [
                'Total Disability Benefit',
                'Partial Disability Benefit',
                'Recurrent Disability',
                'Cover Pause',
                'Concurrent Disability',
                'Inflation Protection',
                'Waiver of Premium Benefit',
                'Rehabilitation Expense Benefit'],
            benefitHeading: 'Income Protection',
            exclusionHeading: 'You are not covered for:',
            exclusionDeclaration: 'There may be additional exclusions applied to your Income Protection depending on the cover options you have chosen. Please refer to the PDS for full details.'
        }
    };
    
    var coverOptionContent = {
        'linkedToCpi': {
            hideOnSelectCover: true,
            heading: '',
            description: 'Adding inflation protection automatically increases how much you are insured for each year in line with the cost of living. As your cover increases, so will the cost.',
            overrideOptions: [
                { value: true, overrideName: 'Include Inflation Protection', shortName: 'Included'},
                { value: false, overrideName: 'Exclude Inflation Potection', shortName: 'Excluded'}
            ]
        },
        'waitingPeriod': {
            heading: 'Choose how long your waiting period will be',
            description: 'With Income Protection you get to choose how many weeks you\'ll wait (your waiting period) before your payments start accruing. You\'ll then receive your first payment one month after this waiting period. </br></br> It\'s worth noting that the longer your waiting period is, the lower your premiums will be. You may also wish to consider your sick, annual and long service leave entitlements before selecting your waiting period. ' +
                         '<p>My waiting period will be: </p>',
            overrideOptions: [
            ]
        },
        'benefitPeriod': {
            heading: 'Choose how many years of protection you want',
            description: 'With Income Protection you get to decide the maximum period you want to receive payments while you are injured or sick. Of course, the longer the period you choose, the more your premium will be.' +
                '<p>I want: </p>',
            overrideOptions: [
                { value: 1, overrideName: '1 years\' protection' },
                { value: 2, overrideName: '2 years\' protection' },
                { value: 5, overrideName: '5 years\' protection' }
            ]
        },
        'occupationDefinition': {
            heading: '<strong>Covering your occupation </strong>',
            description: '<p>With TPD Insurance, you\'ll receive a lump sum payment if a disability prevents you from ever working again in ANY occupation suited to your training, education or experience.</p>' + 
                         '<p>Depending on your circumstances, you can choose to be covered if your disability prevents you from ever working again in the occupation you specialise in \- this is called OWN occupation. You\'ll pay a higher premium for covering your OWN occupation.</p>',
            overrideOptions: [
                { value: 'AnyOccupation', overrideName: 'Cover ANY occupation', shortName: 'Any' },
                { value: 'OwnOccupation', overrideName: 'Cover my OWN occupation', shortName: 'My Own'}
            ]
        }
    };

    var content = {
        review: {
            introText: {
                accept:'<p>Based on the information you\'ve provided, we\'re able to insure you on the spot should you wish to apply.</p>' +
                '<div class="tal-feature-block tal-v-spacing-lg"><p class="p--larger">No medicals, no paperwork and a 30 day cooling off period.</p></div>' +
                '<p>Getting life insurance while you\'re healthy is a good idea, as it can be difficult to get later if medical issues arise.</p>' +
                '<p>By taking out life insurance, you\'re protecting the ones you love and the choices you\'ve worked hard to create.  And you\'re doing it through an insurer that\'s committed to claims. <b>In 2015 alone, payments to our customers exceeded $1 billion.</b> </p>' +
                '<p>You\'ll find a summary of your insurance, how much you\'re insured for and how much it costs below. It\'s easy to make changes, and any fine tuning you do will be automatically reflected in your quote.</p>' +
                '<p>When you\'ve finished reviewing your quote, click  \'{{buttons.review.accept}}\' to pay online now.</p>',

                refer: '<p>Based on the information you\'ve provided, it looks like we need to contact you for more information.</p>' +
                '<p>You\'ll find a summary of your quote below. It\'s easy to make changes, and any fine tuning you do will be automatically reflected in your cover.</p>'                
            }
        },
        purchase: {
            submissionErrorMessage: '<p>Thank you for submitting your quote. Unfortunately, due to technical issues, we\'re unable to process it at this time but we have received your quote and it has been saved.</p>' +
                '<p>Please call <a href="tel:{{ contact.talPhone }}" class="tal-link--strong">{{ contact.talPhone }} </a> to discuss the next steps.</p>'
        }
    };

    this.getContentByReferenceKey = function (referenceKey) {
        return this[referenceKey];
    };

    this.getCompiledContent = function (referenceKey) {
        var rawContent = this.getContentByReferenceKey(referenceKey);
        var compiledContent = $interpolate(rawContent)(sharedContent);
        return compiledContent;
    };

    //Expose content through keys here for lookup
    this['purchase.submissionErrorMessage'] = content.purchase.submissionErrorMessage;
    this['review.introText.accept'] = content.review.introText.accept;
    this['review.introText.refer'] = content.review.introText.refer;
    this['review.buttonText.accept'] = sharedContent.buttons.review.accept;
    this['review.buttonText.refer'] = sharedContent.buttons.review.refer;
    this['coverSelection.plans'] = planBlockContent;
    this['coverSelection.covers'] = coverBlockContent;
    this['coverSelection.coverOptions'] = coverOptionContent;
    this['shared.links'] = sharedContent.links;
    this['shared.contact'] = sharedContent.contact;
    this['consent.dncSelection'] = sharedContent.consent.dncSelection;
    this['consent.termsAgree'] = sharedContent.consent.termsAgree;
    this['contact.exitPhone'] = sharedContent.contact.exitPhone;
    this['contact.talPhone'] = sharedContent.contact.talPhone; 
    this['contact.unsubscribePhone'] = sharedContent.contact.unsubscribePhone; 
    this['contact.confirmationPhone'] = sharedContent.contact.confirmationPhone;
    this.paymentFrequencyPer = paymentFrequencyPer;
    this.planBlockContent = planBlockContent;

    this['infoDialog.DTH.description'] = planBlockContent.DTH.learnMoreModalContent;
    this['infoDialog.TRS.description'] = planBlockContent.TRS.learnMoreModalContent;
    this['infoDialog.IP.description'] = planBlockContent.IP.learnMoreModalContent;
    this['infoDialog.TPS.description'] = planBlockContent.TPS.learnMoreModalContent;

    //The following content requires dynamic content, so expose them via template cache and let angular work its binding magic
    $templateCache.put('content.DTH.reviewDescription.html', planBlockContent.DTH.reviewDescription);
    $templateCache.put('content.TRS.reviewDescription.html', planBlockContent.TRS.reviewDescription);
    $templateCache.put('content.TPS.reviewDescription.html', planBlockContent.TPS.reviewDescription);
    $templateCache.put('content.IP.reviewDescription.html', planBlockContent.IP.reviewDescription);

    $templateCache.put('content.DTH.unselectedDescription.html', planBlockContent.DTH.whenOffDescription);
    $templateCache.put('content.TRS.unselectedDescription.html', planBlockContent.TRS.whenOffDescription);
    $templateCache.put('content.TPS.unselectedDescription.html', planBlockContent.TPS.whenOffDescription);
    $templateCache.put('content.IP.unselectedDescription.html', planBlockContent.IP.whenOffDescription);

});
/// <reference path="bower_components/ngSticky/dist/sticky.min.js" />
/// <reference path="bower_components/ngSticky/dist/sticky.min.js" />
'use strict';

// Karma configuration
// http://karma-runner.github.io/0.10/config/configuration-file.html

module.exports = function (config) {
    config.set({
        // base path, that will be used to resolve files and exclude
        basePath: '',

        // testing framework to use (jasmine/mocha/qunit/...)
        frameworks: ['jasmine'],

        // list of files / patterns to load in the browser
        files: [
            'bower_components/angular/angular.js',
            'bower_components/angular-mocks/angular-mocks.js',
            'bower_components/angular-animate/angular-animate.js',
            'bower_components/angular-busy/dist/angular-busy.js',
            'bower_components/lodash/lodash.js',
            'bower_components/moment/moment.js',
            'bower_components/angular-ui-scroll/dist/ui-scroll.js',
            'bower_components/angular-ui-scrollpoint/dist/scrollpoint.js',
            'bower_components/angular-ui-event/dist/event.js',
            'bower_components/angular-ui-mask/dist/mask.js',
            'bower_components/angular-ui-validate/dist/validate.js',
            'bower_components/angular-ui-indeterminate/dist/indeterminate.js',
            'bower_components/angular-ui-uploader/dist/uploader.js',
            'bower_components/angular-ui-router/release/angular-ui-router.js',
            'bower_components/ui-utils/index.js',
            'bower_components/angular-sanitize/angular-sanitize.js',
            'bower_components/angular-aria/angular-aria.js',
            'bower_components/ng-dialog/js/ngDialog.js',
            'bower_components/angular-ripple/angular-ripple.js',
            'bower_components/enquire/dist/enquire.js',
            'bower_components/ddbreakpoints/lib/dd.breakpoints.js',
            'bower_components/angular-filter-count-to/dist/angular-filter-count-to.min.js',
            'bower_components/angular-highlightjs/build/angular-highlightjs.js',
            'bower_components/angular-spinners/dist/angular-spinners.js',
            'bower_components/angular-toastr/dist/angular-toastr.js',
            'bower_components/ng-slide-down/dist/ng-slide-down.js',
            'bower_components/ngSticky/dist/sticky.min.js',
            'bower_components/ngclipboard/dist/ngclipboard.js',
            'bower_components/intro.js/intro.js',
            'bower_components/angular-resource/angular-resource.min.js',
            'bower_components/angular-intro.js/src/angular-intro.js',
            'bower_components/angular-touch/angular-touch.js',
            'bower_components/angular-ui-select/dist/select.js',
            'bower_components/angular-cookies/angular-cookies.js',
            'bower_components/angular-messages/angular-messages.js',
            'bower_components/angular-recaptcha/release/angular-recaptcha.js',
            'bower_components/oclazyload/dist/ocLazyLoad.min.js',
            'client/**/*.js',
            'client/**/*.html',
            'node_modules/jquery/dist/jquery.js' // Only for testing utility, not included in client vendor scripts.
        ],

        preprocessors: {
            'client/**/*.html': ['ng-html2js'],
            'client/appCustomerPortal/**/!(*.spec|*.templates).js': 'coverage',
            'client/appSelectCover/components/coverBreakdown/!(*.spec|*.templates).js': 'coverage'
        },

        ngHtml2JsPreprocessor: {
            // stripPrefix: 'client/',
            prependPrefix: '/',
            moduleName: 'htmlTemplates'
        },

        // test results reporter to use
        // possible values: 'dots', 'progress', 'junit', 'growl', 'coverage'
        reporters: ['dots', 'coverage'],

        // list of files / patterns to exclude
        exclude: [],

        // web server port
        port: 8080,

        // level of logging
        // possible values: LOG_DISABLE || LOG_ERROR || LOG_WARN || LOG_INFO || LOG_DEBUG
        logLevel: config.LOG_INFO,

        plugins: [
          'karma-teamcity-reporter',
          'karma-phantomjs-launcher',
          'karma-jasmine',
          'karma-coverage',
          'karma-teamcity-reporter',
          'karma-junit-reporter',
          'karma-ng-html2js-preprocessor'
        ],

        // enable / disable watching file and executing tests whenever any file changes
        autoWatch: true,

        // enable / disable colors in the output (reporters and logs)
        colors: true,

        // Start these browsers, currently available:
        // - Chrome
        // - ChromeCanary
        // - Firefox
        // - Opera
        // - Safari (only Mac)
        // - PhantomJS
        // - IE (only Windows)
        browsers: ['PhantomJS'],


        // Continuous Integration mode
        // if true, it capture browsers, run tests and exit
        singleRun: true,

        captureTimeout: 60000,
        browserDisconnectTimeout: 10000,
        browserDisconnectTolerance: 5,
        browserNoActivityTimeout: 20000
    });
};

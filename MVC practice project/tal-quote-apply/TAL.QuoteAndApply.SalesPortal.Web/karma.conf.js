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
          'bower_components/angular-sanitize/angular-sanitize.js',
          'bower_components/angular-resource/angular-resource.js',
          'bower_components/angular-bootstrap/ui-bootstrap-tpls.js',
          'bower_components/lodash/lodash.js',
          'bower_components/angular-bootstrap-show-errors/src/showErrors.js',
          'bower_components/ui-utils/index.js',
          'bower_components/angular-animate/angular-animate.js',
          'bower_components/angular-busy/dist/angular-busy.js',
          'bower_components/angular-ui-event/dist/event.js',
          'bower_components/angular-ui-indeterminate/dist/indeterminate.js',
          'bower_components/angular-ui-mask/dist/mask.js',
          'bower_components/angular-ui-validate/dist/validate.js',
          'bower_components/angular-ui-uploader/dist/uploader.js',
          'bower_components/angular-ui-scroll/dist/ui-scroll.js',
          'bower_components/angular-ui-scrollpoint/dist/scrollpoint.js',
          'bower_components/moment/moment.js',
          'bower_components/angular-spinners/dist/angular-spinners.js',
          'client/app/app.js',
          'client/app/**/*.js',
          'client/app/**/*.html'
        ],

        preprocessors: {
            '**/*.html': 'html2js'
        },

        ngHtml2JsPreprocessor: {
            stripPrefix: 'client/'
        },

        // list of files / patterns to exclude
        exclude: [],

        // web server port
        port: 8080,

        // level of logging
        // possible values: LOG_DISABLE || LOG_ERROR || LOG_WARN || LOG_INFO || LOG_DEBUG
        logLevel: config.LOG_INFO,


        // enable / disable watching file and executing tests whenever any file changes
        autoWatch: false,


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

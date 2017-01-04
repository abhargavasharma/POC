module.exports = function(grunt) {
    var LIVERELOAD_PORT = 35729;
    var lrSnippet = require('connect-livereload')({ port: LIVERELOAD_PORT });
    var _ = require('lodash');
    var path = require('path');

    require('time-grunt')(grunt);
    require('load-grunt-tasks')(grunt);

    var appConfig = {
        appBasicInfo: {
            ngAppName: 'appBasicInfo',
            srcAppClientFolder: 'client/appBasicInfo',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_BasicInfo.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_BasicInfo.cshtml'
        },
        appSelectCover: {
            ngAppName: 'appSelectCover',
            srcAppClientFolder: 'client/appSelectCover',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_SelectCover.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_SelectCover.cshtml'
        },
        appPurchase: {
            ngAppName: 'appPurchase',
            srcAppClientFolder: 'client/appPurchase',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_Purchase.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_Purchase.cshtml'
        },
        appQualification: {
            ngAppName: 'appQualification',
            srcAppClientFolder: 'client/appQualification',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_Qualification.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_Qualification.cshtml'
        },
        appNeedsAnalysis: {
            ngAppName: 'appNeedsAnalysis',
            srcAppClientFolder: 'client/appNeedsAnalysis',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_NeedsAnalysis.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_NeedsAnalysis.cshtml'
        },
        appReview: {
            ngAppName: 'appReview',
            srcAppClientFolder: 'client/appReview',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_Review.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_Review.cshtml'
        },
        appRetrieve: {
            ngAppName: 'appRetrieve',
            srcAppClientFolder: 'client/appRetrieve',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_Retrieve.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_Retrieve.cshtml'
        },
        appExit: {
            ngAppName: 'appExit',
            srcAppClientFolder: 'client/appExit',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_Exit.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_Exit.cshtml'
        },
        appSummary: {
            ngAppName: 'appSummary',
            srcAppClientFolder: 'client/appSummary',
            srcIndexJsFile: 'Views/Shared/GruntPartials/_GruntBuildJsInjectedFiles_Summary.cshtml',
            srcIndexCssFile: 'Views/Shared/GruntPartials/_GruntBuildCssInjectedFiles_Summary.cshtml'
        }
    };

    grunt.initConfig({
        build: appConfig,
        buildless:appConfig,
        buildlessfast:appConfig,
        baseSettings: {
            ngBaseAppName: 'appCustomerPortal',
            distFolder: 'dist',
            distViewsFolder: 'dist/Views',
            distCssFolder: 'dist',
            distJsFolder: 'dist',
            distImagesFolder: 'dist/client/images',
            distFontsFolder: 'dist/client/fonts',
            srcCssClientFolder: 'client/css',
            srcViewsFolder: 'Views',
            srcIndexFile: 'Views/Shared/_Layout.cshtml',
            stagingFolder: '.tmp',
            stagingProcessedFolder: '.tmp/processed',
            srcClientFolder: 'client',
            srcBaseAppClientFolder: 'client/appCustomerPortal'
        },
        appSettings: {},

        clean: {
            distFolder: ["<%= baseSettings.distFolder %>"],
            buildTempFolder: ['.tmp']
        },
        connect: {
            server: {
              options: {
                livereload: true,
                hostname: '0.0.0.0',
                port: 8000
              }
            },
            livereload: {
                options: {
                  middleware: function (connect) {
                    return [
                      connect.static('.tmp'),
                      connect().use(
                        '/bower_components',
                        connect.static('./bower_components')
                      ),
                      connect.static('./')
                    ];
                  }
                }
            },
            dist: {
                options: {
                  base: './'
                }
            }
        },
        copy: {
            processedFilesToDist: {
                files: [
                    { expand: true, cwd: '<%= baseSettings.stagingProcessedFolder %>', src: ['<%= appSettings.ngAppName %>.js'], dest: '<%= baseSettings.distJsFolder %>/', nonull: true },
                    { expand: true, cwd: '<%= baseSettings.stagingProcessedFolder %>', src: ['<%= appSettings.ngAppName %>.generated.css'], dest: '<%= baseSettings.distCssFolder %>/', nonull: true }
                ]
            },
            vendorFilesAndImages: {
                files: [
                    { expand: true, cwd: '<%= baseSettings.stagingProcessedFolder %>', src: ['vendor.js'], dest: '<%= baseSettings.distJsFolder %>', flatten: true, nonull: true },
                    { expand: true, cwd: '<%= baseSettings.stagingProcessedFolder %>', src: ['vendor.css'], dest: '<%= baseSettings.distCssFolder %>', flatten: true, nonull: true },
                    { expand: true, cwd: 'client/images/', src: ['**'], dest: '<%= baseSettings.distImagesFolder %>/' },
                    { expand: true, cwd: 'client/fonts/', src: ['**'], dest: '<%= baseSettings.distFontsFolder %>/' }
                ]
            },
            dotNetRequiredFilesToDist: {
                files: [
                    { expand: true, cwd: 'Views/', src: ['**/*.{html,cshtml,css}', '*.config'], dest: '<%= baseSettings.distFolder %>/Views/' },
                    { expand: true, cwd: 'bin/', src: ['**'], dest: '<%= baseSettings.distFolder %>/bin/' },
                    { expand: true, cwd: './', src: ['*.{asax,ico,ps1}', 'Web*.config'], dest: '<%= baseSettings.distFolder %>/' }
                ]
            },
            fedStaging: {
                files: [
                    { expand: true, src: ['index.html'], dest: '<%= baseSettings.distFolder %>' },
                    { expand: true, cwd: 'client/', src: ['**/*.{html,js,css}'], dest: '<%= baseSettings.distFolder %>/client' },
                    { expand: true, cwd: 'bower_components/', src: ['**/*.{html,js,css}'], dest: '<%= baseSettings.distFolder %>/bower_components' }
                ]
            }
        },
        watch: {
            less: {
                files: [
                    '<%= baseSettings.srcClientFolder %>/**/**/*.less'
                ],
                tasks: ['buildlessfast'],
                options: {
                  livereload: true
                }
            },
            jshint:{
                files: [
                    '<%= baseSettings.srcClientFolder %>/**/**/*.js'
                ],
                tasks: ['jshint:teamcity'],
                options: {
                  livereload: true
                }
            },
            templates: {
                files: [
                    '<%= baseSettings.srcClientFolder %>/styleguide/sections/**/*.html'
                ],
                tasks: ['ngtemplates:styleguide']
            }
        },
        'ftp-deploy': {
          build: {
            auth: {
              host: 'talqa.donlineclients.com',
              port: 21,
              authKey: 'key1'
            },
            src: '<%= baseSettings.distFolder %>',
            dest: '/'
          }
        },
        bowerInstall: {
            addBowerJsAndCssReferences: {
                src: ['<%= baseSettings.srcIndexFile %>'],
                ignorePath: '../../',
                exclude: [],
                fileTypes: {
                    html: {
                        replace: {
                            js: '<script src="/{{filePath}}"></script>',
                            css: '<link rel="stylesheet" href="/{{filePath}}" />'
                        }
                    }
                }
            }
        },

        injector: {
            options: {},
            // Inject application script files into index.html (doesn't include bower)
            scripts: {
                options: {
                    transform: function (filePath) {
                        //console.log('injector transform: ' + filePath);
                        return '<script src="' + filePath + '"></script>';
                    },
                    starttag: '<!-- injector:js -->',
                    endtag: '<!-- endinjector -->'
                },
                files: {
                    '<%= appSettings.srcIndexJsFile %>': [
                        [
                            '{<%= baseSettings.stagingFolder %>,<%= baseSettings.srcBaseAppClientFolder %>}/**/*.js',
                            '{<%= baseSettings.stagingFolder %>,<%= appSettings.srcAppClientFolder %>}/**/*.js',
                            '!**/*.spec.js'
                        ]
                    ]
                }
            },
            styleguide: {
                options: {
                    transform: function (filePath) {
                        //console.log('injector transform: ' + filePath);
                        return '<script src="' + filePath + '"></script>';
                    },
                    starttag: '<!-- injector:js -->',
                    endtag: '<!-- endinjector -->'
                },
                files: {
                    'index.html': [
                        'client/**/**/*.js',
                        '!**/*.spec.js'
                    ]
                }
            },
            // Inject component less into app.less
            less: {
                options: {
                    transform: function (filePath) {
                        filePath = filePath.replace('/client', 'client');
                        return '@import \'' + filePath + '\';';
                    },
                    starttag: '// injector',
                    endtag: '// endinjector'
                },
                files: {
                    '<%= appSettings.srcAppClientFolder %>/<%= appSettings.ngAppName %>.less': [
                        '<%= baseSettings.srcBaseAppClientFolder %>/**/*.less',
                        '<%= baseSettings.srcCssClientFolder %>/**/*.less',
                        '!<%= appSettings.srcAppClientFolder %>/<%= appSettings.ngAppName %>.less',
                        '<%= appSettings.srcAppClientFolder %>/**/*.less'
                    ]
                }
            }
        },

        ngtemplates: {
            options: {
                // This should be the name of your apps angular module
                module: '<%= appSettings.ngAppName %>',
                prefix: '/<%= baseSettings.srcClientFolder %>/',
                quotes: 'single',
                htmlmin: {
                    //collapseBooleanAttributes: true,
                    collapseWhitespace: true,
                    removeAttributeQuotes: false,
                    removeEmptyAttributes: true,
                    removeRedundantAttributes: false,
                    removeScriptTypeAttributes: true,
                    removeStyleLinkTypeAttributes: true
                },
                usemin: '<%= baseSettings.stagingProcessedFolder %>/<%= appSettings.ngAppName %>.js'
            },
            main: {
                cwd: '<%= baseSettings.srcClientFolder %>',
                src: ['{<%= appSettings.ngAppName %>,<%= baseSettings.ngBaseAppName %>}/**/*.html'],
                dest: '<%= baseSettings.stagingFolder %>/<%= appSettings.ngAppName %>-templates.js'
            },
            styleguide: {
                options:{
                    standalone: true,
                    prefix: '',
                    quotes: 'single',
                    module: 'appTemplate'
                },
                src: [
                    '<%= baseSettings.srcClientFolder %>/styleguide/sections/**/*.html',
                    '<%= baseSettings.srcClientFolder %>/styleguide/sections/shared/dummy.html'
                ],
                dest: '<%= baseSettings.srcClientFolder %>/styleguide/sections/shared/templates.js'
            }
        },

        useminPrepare: {
            html: '<%= baseSettings.srcViewsFolder %>/**/*{.html,.cshtml}',
            concat: '{<%= appSettings.ngAppName %>,<%= baseSettings.ngBaseAppName %>}/**/*{.css,.js}',
            options: {
                dest: '<%= baseSettings.stagingProcessedFolder %>',
                root: './'
            }
        },

        usemin: {
            html: ['<%= baseSettings.distViewsFolder %>/**/*{.html,.cshtml}'],
            css: ['<%= baseSettings.distCssFolder %>/*.css'],
            js: ['<%= baseSettings.distJsFolder %>/*.js'],
            options: {
                assetsDirs: ['<%= baseSettings.distCssFolder %>', '<%= baseSettings.distJsFolder %>', '<%= baseSettings.distImagesFolder %>'],
                patterns: {
                    js: [
                        [/(client\/images\/.*?\.(?:gif|jpeg|jpg|png|webp|svg|otf|woff|eot|ttf|woff))/gm, 'Update the JS to reference our revved images']
                    ],
                    css: [
                        [/(client\/images\/.*?\.(?:gif|jpeg|jpg|png|webp|svg|otf|woff|eot|ttf|woff))/gm, 'Update the CSS to reference our revved images']
                    ]
                }
            }
        },

        less: {
            options: { sourceMap: false },
            buildAppCss: {
                files: {
                    'client/css/<%= appSettings.ngAppName %>.generated.css': '<%= appSettings.srcAppClientFolder %>/<%= appSettings.ngAppName %>.less'
                }
            }
        },

        postcss: {
            options: {
                map: true, // inline sourcemaps @TODO: don't forget to turn this off
                processors: [
                    // add vendor prefixes
                    require('autoprefixer')({browsers: [
                        'last 2 versions',
                        'ie 10-11',
                        'not ie <= 9'
                    ]}),

                    // Kirill's icon task -
                    // allow inline coloring of svg background images,
                    // and include each uri only once in the css
                    require('postcss-svgicon')({
                        path: './client/images/svgs/svg-src',
                        prefix: 'TAL_',
                    }),

                    // Optimize the SVGs in the css,
                    // including striping out superfluous attributes and elements, and uri encoding.
                    require('postcss-svgo')({
                        encode: true
                    }),
                    
                    //require('cssnano')() // @TODO: don't forget to turn this on
                ]
            },
            applyAppPostCss: {
                src: 'client/css/<%= appSettings.ngAppName %>.generated.css'
            }
        },

        filerev: {
            images: {
                src: '<%= baseSettings.distImagesFolder %>/*.{jpg,jpeg,gif,png,webp}'
            },
            css: { src: '<%= baseSettings.distCssFolder %>/*.css' },
            js: { src: '<%= baseSettings.distJsFolder %>/*.js' }
        },

        // Allow the use of non-minsafe AngularJS files. Automatically makes it
        // minsafe compatible so Uglify does not destroy the ng references
        ngAnnotate: {
            dist: {
                files: [
                    {
                        expand: true,
                        cwd: '<%= baseSettings.stagingFolder %>/concat',
                        src: '*.js',
                        dest: '<%= baseSettings.stagingFolder %>/concat'
                    }
                ]
            }
        },

        eol: {
            /*
             When injector adds references to js files in _Layout.cshtml, it adds LF chars instead of CRLF which causes issues in usemin
             This step replaces the LF's to CRLF's and makes usemin work properly :)
             */
            crlfInAllHtml: {
                options: {
                    eol: 'crlf',
                    replace: true
                },
                files: [{
                    expand: true,
                    cwd: '<%= baseSettings.distViewsFolder %>',
                    src: ['**/*.cshtml']
                }]
            }
        },

        jshint: {
            files: ['<%= baseSettings.srcClientFolder %>/**/*.js'],
            options: {
                jshintrc: '.jshintrc'
            },
            all: ['<%= baseSettings.srcClientFolder %>/**/*.js'],
            teamcity: {
                options: {
                    reporter: require('jshint-teamcity')
                },
                src: ['<%= baseSettings.srcClientFolder %>/**/*.js']
            }
        },

      
        // Test settings
        karma: {
            teamcity: {
                configFile: 'karma.conf.js',
                singleRun: true,
                reporters: 'teamcity'
            },
            dev: {
                configFile: 'karma.conf.js',
                singleRun: true
            }
        }
    });

    grunt.registerTask('hint', ['jshint:teamcity']);

    grunt.registerTask('test-dev', ['karma:dev']);

    grunt.registerTask('inject', [
        'injector:styleguide',
        'bowerInstall:addBowerJsAndCssReferences',
    ]);

    grunt.registerTask('tpl', [
        'ngtemplates:styleguide'
    ]);

    grunt.registerTask('deploy', [
        'clean:distFolder',
        'copy:fedStaging',
        // 'default', // build and stuff
        'ftp-deploy'
    ]);

    grunt.registerTask('crawlhtml', 'get all the html templates', function() {

        var files = grunt.file.expand(['client/**/*.html']);
        var fileContent = '';
        
        var grapAllFiles = function(){
            var output = [];
             _.forEach(files, function(file) {
                 output.push('/'+file);
            });
            return JSON.stringify(output); 
        }
        fileContent += 'var files = ' + grapAllFiles() + ';\n';
        fileContent += 'module.exports = files;'
        grunt.file.write('casper/configs/files.js', fileContent);
    });
    grunt.registerTask('server', function (target) {
        grunt.task.run([
          'buildlessfast',
          'connect:server',
          'watch'
        ]);
    });

    // Alias for 'server'
    grunt.registerTask('serve', function (target) {
        grunt.task.run(['server']);
    });

    grunt.registerTask('copyFed', function (target) {
        grunt.task.run([
          'copy:fedStaging'
        ]);
    });

    
    grunt.registerTask('default', [
        'jshint:teamcity',
        'karma:teamcity',
        'clean:distFolder',
        'bowerInstall:addBowerJsAndCssReferences',

        'build', //foreach application

        'copy:vendorFilesAndImages', //FYI, this step assumes the last application run, built the vendor.js/css and that's what we copy to dist
        'copy:dotNetRequiredFilesToDist',
        'filerev', //version js, css and image files
        'eol:crlfInAllHtml',
        'usemin' //Update references to use versioned names
    ]);

    grunt.registerTask('nokarma', [
        'jshint:teamcity',
        'clean:distFolder',
        'bowerInstall:addBowerJsAndCssReferences',

        'build', //foreach application

        'copy:vendorFilesAndImages', //FYI, this step assumes the last application run, built the vendor.js/css and that's what we copy to dist
        'copy:dotNetRequiredFilesToDist',
        'filerev', //version js, css and image files
        'eol:crlfInAllHtml',
        'usemin' //Update references to use versioned names
    ]);

    grunt.registerMultiTask('build', 'build ngtemplates for each application', function () {

        grunt.log.writeln('---------------------------');
        grunt.log.writeln(this.target + ': ' + this.data);

        grunt.config.set('appSettings', this.data);

        grunt.task.run([
            'clean:buildTempFolder',
            'injector:scripts', //Inject references to all js files into appropriate locations
            'injector:less', //Inject references to all css files into appropriate locations
            'less:buildAppCss',
            'postcss:applyAppPostCss',
            'useminPrepare', //Auto generates settings for concat, uglify, cssmin
            'ngtemplates:main', //concatenate and register ng templates in the $templateCache
            'concat', //Roll all js and css into one file per type
            'ngAnnotate', //Minsafe angular files
            'uglify', //Min and uglify js files
            'cssmin', //Min the css
            'copy:processedFilesToDist'
        ]);
    });
    grunt.registerMultiTask('buildless', 'build less for each application', function () {

        grunt.log.writeln('---------------------------');
        grunt.log.writeln(this.target + ': ' + this.data);

        grunt.config.set('appSettings', this.data);

        grunt.task.run([
            'injector:less',
            'clean:buildTempFolder',

            'less:buildAppCss',
            'postcss:applyAppPostCss',
            'useminPrepare', //Auto generates settings for concat, uglify, cssmin
            'concat', //Roll all js and css into one file per type
            'cssmin' //Min the css
        ]);
    });

    grunt.registerMultiTask('buildlessfast', 'build less for each application', function () {

        grunt.log.writeln('---------------------------');
        grunt.log.writeln(this.target + ': ' + this.data);

        grunt.config.set('appSettings', this.data);

        grunt.task.run([
            'injector:less',
            'clean:buildTempFolder',
            'less:buildAppCss',
            'postcss:applyAppPostCss'
        ]);
    });
};
module.exports = function(grunt) {

    require('load-grunt-tasks')(grunt);

    grunt.initConfig({
        appSettings: {
            ngAppName: 'salesPortalApp',
            stagingFolder: '.tmp',
            stagingProcessedFolder: '.tmp/processed',
            srcClientFolder: 'client',
            srcAppClientFolder: 'client/app',
            srcCssClientFolder: 'client/css',
            srcViewsFolder: 'Views',
            srcIndexFile: 'Views/Shared/_Layout.cshtml',
            srcIndexJsFile: 'Views/Shared/_GruntBuildJsInjectedFiles.cshtml',
            srcIndexCssFile: 'Views/Shared/_GruntBuildCssInjectedFiles.cshtml',
            distFolder: 'dist',
            distViewsFolder: 'dist/Views',
            distCssFolder: 'dist',
            distJsFolder: 'dist',
            distImagesFolder: 'dist/client/images'
        },

        clean: {
            distFolder: ["<%= appSettings.distFolder %>"],
            buildTempFolder: ['.tmp']
        },

        copy: {
            clientRequiredFilesToDist: {
                files: [
                    { expand: true, cwd: '<%= appSettings.stagingProcessedFolder %>', src: ['*.js'], dest: '<%= appSettings.distJsFolder %>/' },
                    { expand: true, cwd: '<%= appSettings.stagingProcessedFolder %>', src: ['**/*.css'], dest: '<%= appSettings.distCssFolder %>', flatten: true },
                    { expand: true, cwd: 'client/images/', src: ['**'], dest: '<%= appSettings.distImagesFolder %>/' }
                ]
            },
            dotNetRequiredFilesToDist: {
                files: [
                    { expand: true, cwd: 'Views/', src: ['**/*.{html,cshtml,css}', '*.config'], dest: '<%= appSettings.distFolder %>/Views/' },
                    { expand: true, cwd: 'bin/', src: ['**'], dest: '<%= appSettings.distFolder %>/bin/'},
                    { expand: true, cwd: './', src: ['*.{asax,ico,ps1}', 'Web*.config'], dest: '<%= appSettings.distFolder %>/' }
                ]
            },
            bowerComponentRequiredFiles: {
                files: [
                    { expand: true, cwd: './', src: ['bower_components/font-awesome/fonts/*'], dest: '<%= appSettings.distFolder %>/' }
                ]
            },
            fontsToDist: {
                files: [
                    { expand: true, cwd: './', src: ['fonts/**'], dest: '<%= appSettings.distFolder %>' }
                ]
            }
        },

        bowerInstall: {
            addBowerJsAndCssReferences: {
                src: ['<%= appSettings.srcIndexJsFile %>', '<%= appSettings.srcIndexCssFile %>'],
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
                    transform: function(filePath) {
                        return '<script src="' + filePath + '"></script>';
                    },
                    starttag: '<!-- injector:js -->',
                    endtag: '<!-- endinjector -->'
                },
                files: {
                    '<%= appSettings.srcIndexJsFile %>': [
                        [
                            '{<%= appSettings.stagingFolder %>,<%= appSettings.srcAppClientFolder %>}/**/*.js',
                            '!{<%= appSettings.stagingFolder %>,<%= appSettings.srcAppClientFolder %>}/app.js',
                            '!{<%= appSettings.stagingFolder %>,<%= appSettings.srcAppClientFolder %>}/**/*.spec.js'
                        ]
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
                    'client/css/brands/tal/generated/main.less': [
                        '<%= appSettings.srcCssClientFolder %>/common/variables/*.less', //Add variable defaults
                        '<%= appSettings.srcCssClientFolder %>/brands/tal/variables/*.less', //Add variable overrides per brand
                        '<%= appSettings.srcCssClientFolder %>/common/*.less', //Add all common less
                        '<%= appSettings.srcAppClientFolder %>/**/*.less', //Add all less from ng components
                        '<%= appSettings.srcCssClientFolder %>/brands/tal/*.less' //Add brand override less
                    ],
                    'client/css/brands/yb/generated/main.less': [
                        '<%= appSettings.srcCssClientFolder %>/common/variables/*.less', //Add variable defaults
                        '<%= appSettings.srcCssClientFolder %>/brands/yb/variables/*.less', //Add variable overrides per brand
                        '<%= appSettings.srcCssClientFolder %>/common/*.less', //Add all common less
                        '<%= appSettings.srcAppClientFolder %>/**/*.less', //Add all less from ng components
                        '<%= appSettings.srcCssClientFolder %>/brands/yb/*.less' //Add brand override less
                    ]
                }
            }
        },

        ngtemplates: {
            options: {
                // This should be the name of your apps angular module
                module: '<%= appSettings.ngAppName %>',
                prefix: '/<%= appSettings.srcClientFolder %>/',
                htmlmin: {
                    //collapseBooleanAttributes: true,
                    collapseWhitespace: true,
                    removeAttributeQuotes: true,
                    removeEmptyAttributes: true,
                    removeRedundantAttributes: false,
                    removeScriptTypeAttributes: true,
                    removeStyleLinkTypeAttributes: true
                },
                usemin: '<%= appSettings.stagingProcessedFolder %>/app.js'
            },
            main: {
                cwd: '<%= appSettings.srcClientFolder %>',
                src: ['**/*.html'],
                dest: '<%= appSettings.stagingFolder %>/templates.js'
            },
            tmp: {
                cwd: '<%= appSettings.stagingFolder %>',
                src: ['**/*.html'],
                dest: '<%= appSettings.stagingFolder %>/tmp-templates.js'
            }
        },

        useminPrepare: {
            html: '<%= appSettings.srcViewsFolder %>/**/*{.html,.cshtml}',
            options: {
                dest: '<%= appSettings.stagingProcessedFolder %>',
                root: './'
            }
        },

        usemin: {
            html: ['<%= appSettings.distViewsFolder %>/**/*{.html,.cshtml}'],
            css: ['<%= appSettings.distCssFolder %>/*.css'],
            js: ['<%= appSettings.distJsFolder %>/*.js'],
            options: {
                assetsDirs: ['<%= appSettings.distJsFolder %>', '<%= appSettings.distImagesFolder %>'],
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
                    'client/css/brands/tal/generated/app.css': 'client/css/brands/tal/generated/main.less',
                    'client/css/brands/yb/generated/app.css': 'client/css/brands/yb/generated/main.less'
                }
            }
        },

        filerev: {
            images: {
                src: '<%= appSettings.distImagesFolder %>/*.{jpg,jpeg,gif,png,webp}'
            },
            css: {src: '<%= appSettings.distCssFolder %>/*.css'},
            js: {src: '<%= appSettings.distJsFolder %>/*.js'}
        },

        // Allow the use of non-minsafe AngularJS files. Automatically makes it
        // minsafe compatible so Uglify does not destroy the ng references
        ngAnnotate: {
            dist: {
                files: [
                    {
                        expand: true,
                        cwd: '<%= appSettings.stagingFolder %>/concat',
                        src: '*.js',
                        dest: '<%= appSettings.stagingFolder %>/concat'
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
                    replace:true
                },
                files: [{
                    expand: true,
                    cwd: '<%= appSettings.distViewsFolder %>',
                    src: ['**/*.cshtml']
                }]
            }
        },

        jshint: {
            files: ['<%= appSettings.srcAppClientFolder %>/**/*.js'],
            options: {
                jshintrc: '.jshintrc'
            },
            all: [ '<%= appSettings.srcAppClientFolder %>/**/*.js' ],
            teamcity: {
                options: {
                    reporter: require('jshint-teamcity')
                },
                src: [ '<%= appSettings.srcAppClientFolder %>/**/*.js' ]
            }
        },

        watch: {
            less: {
                files: ['<%= appSettings.srcCssClientFolder %>/**/*.less', '<%= appSettings.srcAppClientFolder %>/**/*.less'],
                tasks: ['less'],
                options: {
                    spawn: false
                }
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

    grunt.registerTask('test-dev', ['karma:dev']);

    grunt.registerTask('default', ['jshint:teamcity' , 'karma:teamcity', 'build']);

    grunt.registerTask('buildcss', ['injector:less' , 'less']);

    grunt.registerTask('build', [
        'clean:distFolder', 'clean:buildTempFolder',
        'injector', //Inject references to all js and css files into appropriate locations
        'bowerInstall:addBowerJsAndCssReferences',
        'less:buildAppCss',
        'useminPrepare', //Auto generates settings for concat, uglify, cssmin
        'ngtemplates', //concatenate and register ng templates in the $templateCache
        'concat', //Roll all js and css into one file per type
        'ngAnnotate', //Minsafe angular files
        'uglify', //Min and uglify js files
        'cssmin', //Min the css
        'copy:clientRequiredFilesToDist',
        'copy:dotNetRequiredFilesToDist',
        'copy:fontsToDist',
        'copy:bowerComponentRequiredFiles',
        'filerev', //version js, css and image files
        'eol:crlfInAllHtml',
        'usemin' //Update references to use versioned names
    ]);

};
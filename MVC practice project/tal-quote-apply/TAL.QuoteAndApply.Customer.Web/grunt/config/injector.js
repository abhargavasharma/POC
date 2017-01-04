module.exports = {
	options: {
		lineEnding:'\r\n'
	},
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
			'<%= baseSettings.partialsFolder %>/_GruntBuildJsInjectedFiles_<%= buildAppName %>.cshtml': [
				[
					'<%= baseSettings.srcBaseAppClientFolder %>/**/*.js',
					'<%= baseSettings.srcClientFolder %>/<%= buildAppName %>/**/*.js',
					'!<%= baseSettings.srcBaseAppClientFolder %>/**/*.spec.js',
					'!<%= baseSettings.srcClientFolder %>/<%= buildAppName %>/**/*.spec.js'
				]
			]
		}
	},
	css: {
		options: {
			transform: function (filePath) {
				console.log('css injector transform: ' + filePath);
				return '<link rel="stylesheet" href="' + filePath + '" />';
			},
			starttag: '<!-- injector:css -->',
			endtag: '<!-- endinjector -->'
		},
		files: {
			'<%= baseSettings.partialsFolder %>/_GruntBuildCssInjectedFiles_<%= buildAppName %>.cshtml': [
				[
					'<%= baseSettings.srcAssetsFolder %>/<%= buildAppName %>.generated.css'
				]
			]
		}
	},
	styleguidecss: {
		options: {
			transform: function (filePath) {
				console.log('css injector transform: ' + filePath);
				return '<link rel="stylesheet" href="' + filePath + '" />';
			},
			starttag: '<!-- injector:css -->',
			endtag: '<!-- endinjector -->'
		},
		files: {
			'<%= STYLEGUIDE_PATH.INDEX %>': [
				'<%= baseSettings.srcClientFolder %>/**/*.css'
			]
		}
	},
	styleguidejs: {
		options: {
			transform: function (filePath) {
				//console.log('injector transform: ' + filePath);
				filePath = filePath.replace('/.clientTemp', '.clientTemp');
				return '<script src="' + filePath + '"></script>';
			},
			starttag: '<!-- injector:js -->',
			endtag: '<!-- endinjector -->'
		},
		files: {
			'<%= STYLEGUIDE_PATH.INDEX %>': [
				'<%= baseSettings.srcClientFolder %>/**/*.js',
				'!<%= baseSettings.srcClientFolder %>/**/*.spec.js'
			]
		}
	},
	// Inject component less into app.less
	less: {
		options: {
			transform: function (filePath) {
				// TODO: this dir ref shouldn't be hardcoded
				filePath = filePath.replace('/client', 'client');
				return '@import \'' + filePath + '\';';
			},
			starttag: '// injector',
			endtag: '// endinjector'
		},
		files: {
			'<%= baseSettings.srcClientFolder %>/<%= buildAppName %>/<%= buildAppName %>.less': [

				// Global less files. The overrides are the per-brand variables.
				'<%= SERVE_PATH.STYLES_DEFAULT %>/**/*.less',
				'<%= SERVE_PATH.STYLES_OVERRIDES %>/**/*.less',
				'<%= SERVE_PATH.STYLES_SHARED %>/**/*.less',

				// Styles for the core app
				'<%= SERVE_PATH.APP_CORE %>/**/*.less',

				// Styles for the specific app being built
				'<%= PATH.CLIENT_TEMP %>/<%= buildAppName %>/**/*.less',
				'!<%= PATH.CLIENT_TEMP %>/<%= buildAppName %>/<%= buildAppName %>.less'
			]
		}
	},
};
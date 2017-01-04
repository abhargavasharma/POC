module.exports = {
	appLess: {
		files: [
			'client/css/**/**/*.less',
			'<%= baseSettings.srcBaseAppClientFolder %>/**/**/*.less',
			'<%= baseSettings.srcClientFolder %>/<%= buildAppName %>/**/**/*.less'
		],
		tasks: ['buildSingleAppLess:<%= buildAppName %>'],
		options: {
			livereload: false
		}
	},
	less: {
		files: [
			'<%= baseSettings.srcClientFolder %>/**/**/*.less'
		],
		tasks: ['fedBuildAll'],
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
	},

	// WHITELABELING
	changesInBaseClient: {
		files: [ '<%= PATH.CLIENT_BASE %>/**/*' ],
		tasks: [
			'newer:copy:baseClientToTemp',
			'copy:brandClientToTemp',
			'replace:dist'
		]
	},
	changesInBrandClient: {
		files: [
			'<%= PATH.CLIENT_BRAND %>/**/*',
		],
		tasks: [
			'copy:brandClientToTemp',
			'replace:dist'
		]
	}
};

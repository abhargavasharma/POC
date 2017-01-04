module.exports = function(grunt, options) {

	var brand = options.brandSettings.brand;

	return {
		options: {
			// This should be the name of your apps angular module
			module: '<%= buildAppName %>',
			prefix: '/',
			quotes: 'single',

			// Replace '/clientTemp' with 'client' in the template keys (these are the full paths by default).
			htmlmin: {
				//collapseBooleanAttributes: true,
				collapseWhitespace: true,
				removeAttributeQuotes: false,
				removeEmptyAttributes: true,
				removeRedundantAttributes: false,
				removeScriptTypeAttributes: true,
				removeStyleLinkTypeAttributes: true
			},
			usemin: '<%= baseSettings.stagingProcessedFolder %>/<%= buildAppName %>.js'
		},
		main: {
			src: [
				'{<%= baseSettings.srcClientFolder %>/<%= buildAppName %>,<%= baseSettings.srcClientFolder %>/<%= baseSettings.ngBaseAppName %>}/**/*.html'
			],
			dest: '<%= baseSettings.stagingFolder %>/<%= buildAppName %>-templates.js'
		},
		styleguide: {
			options:{
				standalone: true,
				prefix: '/',
				quotes: 'single',
				module: 'appTemplate'
			},
			src: [
				'<%= PATH.CLIENT_TEMP %>/app*/**/*.html',
				'<%= PATH.CLIENT_TEMP %>/styleguide/sections/**/*.html',
				'<%= PATH.CLIENT_TEMP %>/styleguide/sections/shared/dummy.html',
				// includedTemplates
			],
			dest: '<%= PATH.CLIENT_TEMP %>/styleguide/sections/shared/templates.js'
		}
	};
};

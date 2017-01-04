module.exports = {
	app: {
		files: [
			{ dest: '.tmp/concat/<%= buildAppName %>.js', src: [
				'<%= baseSettings.srcBaseAppClientFolder %>/**/*.js',
				'<%= baseSettings.srcClientFolder %>/<%= buildAppName %>/**/*.js',
				'<%= baseSettings.stagingFolder %>/<%= buildAppName %>-templates.js',
				'!<%= baseSettings.srcBaseAppClientFolder %>/**/*.spec.js',
				'!<%= baseSettings.srcClientFolder %>/<%= buildAppName %>/**/*.spec.js'
			]},
			{ dest: '.tmp/concat/<%= buildAppName %>.css', src: [
				'<%= baseSettings.srcClientFolder %>/assets/<%= buildAppName %>.generated.css'
			]}
		]
	}
};

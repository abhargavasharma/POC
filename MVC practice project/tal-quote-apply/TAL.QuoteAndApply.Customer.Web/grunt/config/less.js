module.exports = {
	options: { sourceMap: false },
	buildAppCss: {
		files: {
			'<%= baseSettings.generatedCssFolder %>/<%= buildAppName %>.generated.css': '<%= baseSettings.srcClientFolder %>/<%= buildAppName %>/<%= buildAppName %>.less'
		}
	}
};

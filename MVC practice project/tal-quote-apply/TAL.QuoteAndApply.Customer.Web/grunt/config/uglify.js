module.exports = {
	//TODO: can probably refactor to use useminPrepare (but only target the apps js cshtml file)
	app: {
		files: {
			'.tmp/concat/<%= buildAppName %>.min.js': ['.tmp/concat/<%= buildAppName %>.js']
		}
	}
};

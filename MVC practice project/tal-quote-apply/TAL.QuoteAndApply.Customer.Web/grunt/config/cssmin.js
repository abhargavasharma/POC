module.exports = {
	//TODO: can probably refactor to use useminPrepare (but only target the apps css cshtml file)
	app: {
		files: {
			'.tmp/concat/<%= buildAppName %>.generated.css': ['.tmp/concat/<%= buildAppName %>.css']
		}
	}
};

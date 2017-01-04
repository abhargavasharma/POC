module.exports = {
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
};

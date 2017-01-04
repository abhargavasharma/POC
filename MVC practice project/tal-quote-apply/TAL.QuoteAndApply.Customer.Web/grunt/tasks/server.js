module.exports = function(grunt) {
	// DEPROCATED: Use serve or fedServe instead.
	grunt.registerTask('server', 'Serve the FED Styleguide to localhost and update on changes', [
		'copy:baseClientToTemp',
		'copy:brandClientToTemp',

		'replace:dist',
		'replace:nginclude',

		'fedBuildAll',
		'connect:server',
		'watch'
	]);
};

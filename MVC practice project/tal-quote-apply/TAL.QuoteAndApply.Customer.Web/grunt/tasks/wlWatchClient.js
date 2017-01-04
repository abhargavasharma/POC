module.exports = function(grunt) {
	// Aliases for 'fedBuildAll'
	grunt.registerTask('wlWatchClient', 'Watches changes in the client dirs and zips the base and brand dirs into a temp dir from which to serve', [
		'concurrent:watchAndMergeClientToTemp'
	]);
};

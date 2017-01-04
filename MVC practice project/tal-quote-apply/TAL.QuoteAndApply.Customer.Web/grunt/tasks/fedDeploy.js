module.exports = function(grunt) {
	grunt.registerTask('fedDeploy', 'Deploy FED Styelguide to the configured preview server', [
		'clean:distFolder',
		'copy:fedStaging',
		'ftp-deploy'
	]);
};

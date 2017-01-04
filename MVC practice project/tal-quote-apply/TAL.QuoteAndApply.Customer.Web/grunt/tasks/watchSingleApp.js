module.exports = function(grunt) {
	grunt.registerTask('watchSingleApp', 'Watches a single app', function(buildAppName) {
		grunt.config.set('buildAppName', buildAppName);
		grunt.task.run('watch:appLess:' + buildAppName);
	});
};

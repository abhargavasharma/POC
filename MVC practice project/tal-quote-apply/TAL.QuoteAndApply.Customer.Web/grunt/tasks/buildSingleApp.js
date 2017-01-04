module.exports = function(grunt) {
	grunt.registerTask('buildSingleApp', 'Builds a single app', function(buildAppName) {
		grunt.config.set('buildAppName', buildAppName);

		grunt.task.run('injector:less');
		grunt.task.run('less:buildAppCss');
		grunt.task.run('postcss:app');
		grunt.task.run('ngtemplates:main');
		grunt.task.run('concat:app');
		grunt.task.run('ngAnnotate');
		grunt.task.run('uglify:app');
		grunt.task.run('cssmin:app');
		grunt.task.run('injector:scripts');
		grunt.task.run('injector:css');
		grunt.task.run('copy:processedAppFilesToDist');
	});
};

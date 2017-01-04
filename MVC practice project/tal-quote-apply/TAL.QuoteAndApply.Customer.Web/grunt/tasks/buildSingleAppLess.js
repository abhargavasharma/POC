module.exports = function(grunt) {
	grunt.registerTask('buildSingleAppLess', 'Builds Less for a single app', function(buildAppName) {
		grunt.config.set('buildAppName', buildAppName);
		grunt.task.run('injector:less');
		grunt.task.run('less:buildAppCss');
	});
};

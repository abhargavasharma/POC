module.exports = function(grunt) {
	grunt.registerTask('fedBuildSingle', 'Builds FED styleguide for one app', function(buildAppName) {
		grunt.config.set('buildAppName', buildAppName);

		grunt.task.run('injector:less');
		grunt.task.run('less:buildAppCss');
		grunt.task.run('postcss:app');
	});
};

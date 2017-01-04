module.exports = function(grunt) {
	grunt.registerTask('fedTpl', 'Generate the template cache for FED templates', [ 
		'ngtemplates:styleguide'
	]);
};

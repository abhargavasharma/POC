module.exports = function(grunt) {
	grunt.registerTask('fedInject', 'Inject all js and css assets into styleguides index.js', [
		'injector:styleguide',
		'bowerInstall:addBowerJsAndCssReferences',
	]);
};

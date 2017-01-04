module.exports = function(grunt) {
	grunt.registerTask('buildVendorCssAndJs', [
		'useminPrepare',
		'concat:generated',
		'uglify:generated',
		'cssmin:generated'
	]);
};

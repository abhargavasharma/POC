module.exports = function(grunt) {
	grunt.registerTask('default', [
	    //Verify build
	    'clean:clientTest',
	    'copy:baseClientToTest',
	    'copy:brandClientToTest',
	    'jshint:teamcity',
	    'karma:teamcity',
	    'clean:clientTest',

	    'copy:baseClientToTemp',
	    'copy:brandClientToTemp',

	    'buildFullApp'
	]);
};

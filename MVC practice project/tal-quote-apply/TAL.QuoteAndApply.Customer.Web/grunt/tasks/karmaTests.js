module.exports = function(grunt) {
	// Aliases for 'fedBuildAll'
	grunt.registerTask('karmaTests', 'Runs Karma in dev mode', 
		[
			'copy:baseClientToTest',
		    'copy:brandClientToTest',
		    'jshint:teamcity',
		    'karma:dev',
		    'clean:clientTest',
	    ]
	);
};

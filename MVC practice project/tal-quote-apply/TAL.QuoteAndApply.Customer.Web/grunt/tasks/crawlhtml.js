var _ = require('lodash');

module.exports = function(grunt) {
	grunt.registerTask('crawlhtml', 'Get all the html templates', function() {
		var files = grunt.file.expand(['client/**/*.html']);
		var fileContent = '';
		
		var grapAllFiles = function(){
			var output = [];
			 _.forEach(files, function(file) {
				 output.push('/'+file);
			});
			return JSON.stringify(output); 
		};

		fileContent += 'var files = ' + grapAllFiles() + ';\n';
		fileContent += 'module.exports = files;';
		grunt.file.write('casper/configs/files.js', fileContent);
	});
};

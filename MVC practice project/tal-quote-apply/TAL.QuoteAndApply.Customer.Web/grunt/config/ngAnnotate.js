// Allow the use of non-minsafe AngularJS files. Automatically makes it
// minsafe compatible so Uglify does not destroy the ng references

module.exports = {
	dist: {
		files: [
			{
				expand: true,
				cwd: '<%= baseSettings.stagingFolder %>/concat',
				src: '<%= buildAppName %>.js',
				dest: '<%= baseSettings.stagingFolder %>/concat'
			}
		]
	}
};

module.exports = {
	files: ['<%= baseSettings.srcClientFolder %>/**/*.js'],
	options: {
		jshintrc: '.jshintrc'
	},
	all: ['<%= baseSettings.srcClientFolder %>/**/*.js'],
	teamcity: {
		options: {
			reporter: require('jshint-teamcity')
		},
		src: ['<%= baseSettings.srcClientFolder %>/**/*.js']
	}
};

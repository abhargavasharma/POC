module.exports = {
	server: {
		options: {
			livereload: true,
			hostname: '0.0.0.0',
			port: '<%= port %>'
		}
	},
	livereload: {
		options: {
			middleware: function (connect) {
			return [
				connect.static('.tmp'),
				connect().use(
				'/bower_components',
				connect.static('./bower_components')
				),
				connect.static('./')
			];
			}
		}
	},
	dist: {
		options: {
			base: './'
		}
	}
};

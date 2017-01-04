module.exports = {
	build: {
		auth: {
			host: 'talqa.donlineclients.com',
			port: 21,
			authKey: 'key1'
		},
		src: '<%= baseSettings.distFolder %>',
		dest: '/'
	}
};

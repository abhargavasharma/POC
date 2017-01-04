module.exports = {
	// WHITELABELING
	zipBaseWithBrand: {
		tasks: [
			'watch:changesInBaseClient',
			'watch:changesInBrandClient'
		],
		options: {
			limit: 6,
			logConcurrentOutput: true
		}
	}
};

module.exports = function(grunt) {
	grunt.registerTask('fedBuildAll', 'Builds FED styleguide', function() {
		grunt.task.run([
			// Clean old temp files
			//'clean:buildTempFolder',

			// Collect base + brand icons
			// 'clean:tempBrandDir',
			// 'copy:brandAssets',

			//Build each app
			'fedBuildSingle:appBasicInfo',
			'fedBuildSingle:appSelectCover',
			'fedBuildSingle:appQualification',
			'fedBuildSingle:appReview',
			'fedBuildSingle:appPurchase',
			'fedBuildSingle:appNeedsAnalysis',
			'fedBuildSingle:appExit',
			'fedBuildSingle:appSummary',
			'fedBuildSingle:appRetrieve',

			'injector:styleguidejs',
			'injector:styleguidecss'
		]);
	});
};

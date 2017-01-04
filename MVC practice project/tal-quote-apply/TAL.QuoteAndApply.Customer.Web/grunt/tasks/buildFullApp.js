module.exports = function(grunt) {
	grunt.registerTask('buildFullApp', [
		//Clean
		'clean:distFolder',
		'clean:buildTempFolder',
		'clean:tempBrandDir',
		'clean:clientTemp',

		'copy:baseClientToTemp',
		'copy:brandClientToTemp',

		'replace:dist',
		'replace:nginclude',

		// Merge base and brand assets
		'copy:brandAssets',

		//Build each app
		'buildSingleApp:appBasicInfo',
		'buildSingleApp:appSelectCover',
		'buildSingleApp:appQualification',
		'buildSingleApp:appReview',
		'buildSingleApp:appPurchase',
		'buildSingleApp:appNeedsAnalysis',
		'buildSingleApp:appExit',
		'buildSingleApp:appSummary',
		'buildSingleApp:appRetrieve',

		//Vendor
		'bowerInstall:addBowerJsAndCssReferences',
		'buildVendorCssAndJs',

		//Dist
		'copy:vendorFilesAndImages',
		'copy:dotNetRequiredFilesToDist',
		//'filerev', //version js, css and image files
		'eol:crlfInAllHtml',
		'usemin', //Update references to use versioned names

		//Tidy
		'clean:buildTempFolder',
		'clean:tempBrandDir'
	]);
};

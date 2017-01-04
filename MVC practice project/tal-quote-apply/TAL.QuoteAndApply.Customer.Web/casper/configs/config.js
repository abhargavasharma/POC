// Casper configuration
// ====================

// Don't touch unless you know what you are doing.


var fs = require( 'fs' );

module.exports = {
	rebase: casper.cli.get( "rebase" ),
	// SlimerJS needs explicit knowledge of this Casper, and lots of absolute paths
	casper: casper,
	libraryRoot: fs.absolute( fs.workingDirectory + '/node_modules/phantomcss' ),
	screenshotRoot: fs.absolute( fs.workingDirectory + '/screenshots' ),
	failedComparisonsRoot: fs.absolute( fs.workingDirectory + '/demo/failures' ),
	addLabelToFailedImage: true,
 	captureWaitEnabled: false,
	currentDirectory : fs.workingDirectory
};
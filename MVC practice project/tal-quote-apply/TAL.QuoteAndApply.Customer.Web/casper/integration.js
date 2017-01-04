// Integration testing
// =================

// Checks the validity of 
// every component defined in the modules folder. 
// and ensure that all the forbidden  classes are not included.


var phantomcss = require( 'phantomcss' );
var config = require('configs/config');
var _ = require('underscore');
var testModule = require('configs/modulesCheck')
var _ = require('underscore');
var pages = require('configs/pages');
var files = require('configs/files');

var forbiddenClasses = require('modules/forbiddenClasses');

var options = casper.cli.options;
var locations = [];
console.log(JSON.stringify(options));

phantomcss.init( config );


casper.on( 'remote.message', function ( msg ) {
	this.echo( 'remote.message' + msg );
} )

casper.on( 'error', function ( err ) {
	this.die( "PhantomJS has an error: " + err );
} );

casper.on( 'resource.error', function ( err ) {
	casper.log( 'Resource load error: ' + err, 'warning' );
} );

var checkModule = function(selector, pageName){
	if(casper.exists(selector)){
		testModule[selector](selector, pageName);
	} 
}

var getPagesFromFiles = function(){
	var pages = [];
	var prefix = 'http://localhost:8000';
	_.each(files, function(file, i){
		pages.push({
			url : prefix + file,
			name: file
		});
	});
	return pages;
}
locations = (options.files)?getPagesFromFiles():pages;


_.each(locations, function(page){
	casper.test.begin( 'Testing TAL app ' + page.name, function ( test ) {
		
		// Opening the page
		// --------------------
		console.log('opening page ' + page.name );
		// --------------------
		
		casper.start( page.url);
		casper.then(function(){
			console.log(casper.page.injectJs("casper/libs/jquery.js"));
		});
		// Setting up the tests
		// --------------------
		console.log('setting tests');
		// --------------------

		casper.then(function(){
			// Looping through all the module tests
			_.each(testModule, function(item, selector){
				checkModule(selector, page.name);
			});

			// looping through all the forbidden classes
			_.each(forbiddenClasses, function(className){
				casper.test.assert(
	        casper.exists('.' + className) === false, 
	        '"' + className + '" should not be in ' + page.name
	      );
			})
		});
	
		// Running  the tests
		// --------------------
		console.log('running tests');
		// --------------------
		
		casper.run( function () {
			console.log( '\nTHE END.' );
			// phantomcss.getExitStatus() // pass or fail?
			casper.test.done();
		} );
	});
});
module.exports = function(grunt) {
	var LIVERELOAD_PORT = 35729;
	var lrSnippet = require('connect-livereload')({ port: LIVERELOAD_PORT });
	var path = require('path');

	require('time-grunt')(grunt);


	// ----  Process brand options processing --------------------
	
	// Passed via the --allBrands flag when running any grunt command
	var buildAllBrands = grunt.cli.options.allBrands;
	
	// Passed via the --brand flag when running any grunt command
	var brand = grunt.cli.options.brand || 'tal';

	
	require('load-grunt-config')(grunt, {
		configPath: path.join(process.cwd(), 'grunt/config'),
		jitGrunt: {
			customTasksDir: 'grunt/tasks',
			staticMappings: {
				// Badly behaved plugins which can't be mapped automatically
				ngtemplates: 'grunt-angular-templates',
				useminPrepare: 'grunt-usemin'
			}
		},
		data: {
			port: grunt.option('port') || '8000',
			buildAppName: '', //dynamically replace this with each app being built
			baseSettings: {
				// ---- Common or Root directories ------------------------
				projectDir: __dirname,
				ngBaseAppName: 'appCustomerPortal',
				srcClientFolder: 'client/' + brand,
				srcBaseAppClientFolder: 'client/' + brand + '/appCustomerPortal',
				srcViewsFolder: 'Views',
				srcIndexFile: 'Views/Shared/_Layout.cshtml',

				// ---- Core Assets (for the base 'bland' brand) ----------
				srcAssetsFolder: 'client/' + brand + '/assets',
				srcCssFolder: 'client/' + brand + '/assets/less',
				srcIconFolder: 'client/' + brand + '/assets/icons',
				srcImagesFolder: 'client/' + brand + '/assets/images',
				srcFontsFolder: 'client/' + brand + '/assets/fonts',
				srcFaviconsFolder: 'client/' + brand + '/assets/favicons',

				// ---- Brand Assets (for overriding base) ----------------
				srcBrandsFolder: 'client/' + brand + '/assets/brands',

				// ---- Temp/working/processing directories ----------------
				generatedCssFolder: 'client/' + brand + '/assets',
				stagingFolder: '.tmp',
				tempBrandDir: '.brandtmp',
				tempBrandImagesDir: '.brandtmp/images',
				tempBrandFontsDir: '.brandtmp/fonts',
				tempBrandFaviconsDir: '.brandtmp/favicons',
				stagingProcessedFolder: '.tmp/processed',

				// ---- dev directories -------------------------------------
				partialsFolder: 'Views/Shared/GruntPartials', 

				// ---- Distribution ----------------------------------------
				distFolder: 'dist/' + brand,
				distViewsFolder: 'dist/' + brand + '/Views',
				distCssFolder: 'dist/' + brand + '/assets',
				distJsFolder: 'dist/' + brand + '/assets',
				distImagesFolder: 'dist/' + brand + '/assets/images',
				distFontsFolder: 'dist/' + brand + '/assets/fonts',
				distFaviconsFolder: 'dist/' + brand + '/assets/favicons'
			},

			PATH: {
				// ---- Distribution ----------------------------------------
				CLIENT_BASE: 'clientBase',
				CLIENT_BRAND: 'clientBrands/' + brand,
				CLIENT_TEMP: 'client/' + brand,
				CLIENT_TEST_TEMP: 'client',
			},

			STYLEGUIDE_PATH: {
				INDEX: 'index.html'
			},

			SERVE_PATH: {
				APP_CORE: 'client/' + brand + '/appCustomerPortal',

				ASSETS: 'client/' + brand + '/assets',
				STYLES_DEFAULT: 'client/' + brand + '/assets/less/defaults',
				STYLES_OVERRIDES: 'client/' + brand + '/assets/less/overrides',
				STYLES_SHARED: 'client/' + brand + '/assets/less/shared',
				FONTS: 'client/' + brand + '/assets/fonts',
				ICONS: 'client/' + brand + '/assets/icons',
				IMAGES: 'client/' + brand + 'assets/images',
				FAVICONS: 'client/assets/favicons'
			},

			// Configuration for working with specific brands.
			brandSettings: {
				brand: brand,
				brandSrcDir: 'client/' + brand + '/assets/',
				brandSrcCssFolder: 'client/' + brand + '/assets/less',
				brandSrcIconsFolder: 'client/' + brand + '/assets/icons',
				brandSrcImagesFolder: 'client/' + brand + '/assets/images',
				brandSrcFontsFolder: 'client/' + brand + '/assets/fonts',
				brandSrcFaviconsFolder: 'client/assets/favicons',
			},

			apps: [
				'appBasicInfo',
				'appExit',
				'appNeedsAnalysis',
				'appPurchase',
				'appQualification',
				'appRetrieve',
				'appReview',
				'appSelectCover',
				'appSummary'
			]
		}
	});
};
module.exports = {
	processedAppFilesToDist: {
		files: [
			{ expand: true, cwd: '<%= baseSettings.stagingFolder %>/concat', src: ['<%= buildAppName %>.min.js'], dest: '<%= baseSettings.distJsFolder %>/', nonull: true },
			{ expand: true, cwd: '<%= baseSettings.stagingFolder %>/concat', src: ['<%= buildAppName %>.generated.css'], dest: '<%= baseSettings.distCssFolder %>/', nonull: true }
		]
	},
	vendorFilesAndImages: {
		files: [
			{ expand: true, cwd: '<%= baseSettings.stagingFolder %>/concat/assets', src: ['vendor.js'], dest: '<%= baseSettings.distJsFolder %>', flatten: true, nonull: true },
			{ expand: true, cwd: '<%= baseSettings.stagingFolder %>/concat/assets', src: ['vendor.css'], dest: '<%= baseSettings.distCssFolder %>', flatten: true, nonull: true },
			{ expand: true, cwd: '<%= baseSettings.tempBrandImagesDir %>', src: ['**'], dest: '<%= baseSettings.distImagesFolder %>/' },
			{ expand: true, cwd: '<%= baseSettings.tempBrandFontsDir %>', src: ['**'], dest: '<%= baseSettings.distFontsFolder %>/' },
			{ expand: true, cwd: '<%= baseSettings.tempBrandFaviconsDir %>', src: ['**'], dest: '<%= baseSettings.distFaviconsFolder %>/' }
		]
	},
	dotNetRequiredFilesToDist: {
		files: [
			{ expand: true, cwd: 'Views/', src: ['**/*.{html,cshtml,css}', '*.config'], dest: '<%= baseSettings.distFolder %>/Views/' },
			{ expand: true, cwd: 'bin/', src: ['**'], dest: '<%= baseSettings.distFolder %>/bin/' },
			{ expand: true, cwd: './', src: ['*.{asax,ico,ps1}', 'Web*.config'], dest: '<%= baseSettings.distFolder %>/' }
		]
	},
	fedStaging: {
		files: [
			{ expand: true, src: ['index.html'], dest: '<%= baseSettings.distFolder %>' },
			{ expand: true, cwd: 'client/', src: ['**/*.{html,js,css,otf,png,jpg}'], dest: '<%= baseSettings.distFolder %>/client' },
			{ expand: true, cwd: 'bower_components/', src: ['**/*.{html,js,css}'], dest: '<%= baseSettings.distFolder %>/bower_components' }
		]
	},



	// Copy (svg) icons, images and fonts
	brandAssets: {
		files: [
			{
				expand: true,
				cwd: '<%= baseSettings.srcAssetsFolder %>',
				src: ['**/*.{svg,jpg,png,otf}'],
				dest: '<%= baseSettings.tempBrandDir %>'
			},
			{
				expand: true,
				cwd: '<%= brandSettings.brandSrcDir %>',
				src: ['**/*.{svg,jpg,png,otf}'],
				dest: '<%= baseSettings.tempBrandDir %>'
			}
		]
	},

	// Whitelabeling
	baseClientToTemp: {
		files: [
			{ 
				expand: true,
				cwd: '<%= PATH.CLIENT_BASE %>',
				src: ['**/*'],
				dest: '<%= PATH.CLIENT_TEMP %>'
			}
		]
	},
	brandClientToTemp: {
		files: [
			{ 
				expand: true,
				cwd: '<%= PATH.CLIENT_BRAND %>',
				src: ['**/*'],
				dest: '<%= PATH.CLIENT_TEMP %>'
			}
		]
	},
	baseClientToTest: {
		files: [
			{ 
				expand: true,
				cwd: '<%= PATH.CLIENT_BASE %>',
				src: ['**/*'],
				dest: '<%= PATH.CLIENT_TEST_TEMP %>'
			}
		]
	},
	brandClientToTest: {
		files: [
			{ 
				expand: true,
				cwd: '<%= PATH.CLIENT_BRAND %>',
				src: ['**/*'],
				dest: '<%= PATH.CLIENT_TEST_TEMP %>'
			}
		]
	}
};

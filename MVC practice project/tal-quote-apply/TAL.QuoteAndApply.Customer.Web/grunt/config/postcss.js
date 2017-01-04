module.exports = function(grunt, options) {
	var iconsPath = options.baseSettings.srcIconFolder;

	return {
		options: {
			map: false, // inline sourcemaps @TODO: don't forget to turn this off
			processors: [
				// add vendor prefixes
				require('autoprefixer')({browsers: [
					'last 2 versions',
					'ie 10-11',
					'not ie <= 9'
				]}),

				// Kirill's icon task -
				// allow inline coloring of svg background images,
				// and include each uri only once in the css
				require('postcss-svgicon')({
					path: iconsPath,
					prefix: 'ICON_',
				}),

				// Optimize the SVGs in the css,
				// including striping out superfluous attributes and elements, and uri encoding.
				require('postcss-svgo')({
					encode: true
				}),
				
				//require('cssnano')() // @TODO: don't forget to turn this on
			]
		},
		app: {
			src: [
				'<%= baseSettings.srcAssetsFolder %>/<%= buildAppName %>.generated.css'
			]
		}
	};
};

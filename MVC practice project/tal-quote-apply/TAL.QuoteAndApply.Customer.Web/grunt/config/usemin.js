module.exports = {
	html: ['<%= baseSettings.distViewsFolder %>/**/*{.html,.cshtml}'],
	css: ['<%= baseSettings.distCssFolder %>/*.css'],
	js: ['<%= baseSettings.distJsFolder %>/*.js'],
	options: {
		basedir: '<%= baseSettings.distFolder %>',
		assetsDirs: ['<%= baseSettings.distCssFolder %>', '<%= baseSettings.distJsFolder %>', '<%= baseSettings.distImagesFolder %>'],
		patterns: {
			js: [
				[/(assets\/images\/.*?\.(?:gif|jpeg|jpg|png|webp|svg|otf|woff|eot|ttf|woff))/gm, 'Update the JS to reference our revved images']
			],
			css: [
				[/(assets\/images\/.*?\.(?:gif|jpeg|jpg|png|webp|svg|otf|woff|eot|ttf|woff))/gm, 'Update the CSS to reference our revved images']
			]
		}
	}
};

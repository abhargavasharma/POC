module.exports = {
	addBowerJsAndCssReferences: {
		src: ['<%= baseSettings.srcIndexFile %>'],
		ignorePath: '../../',
		exclude: [],
		fileTypes: {
			html: {
				replace: {
					js: '<script src="{{filePath}}"></script>',
					css: '<link rel="stylesheet" href="{{filePath}}" />'
				}
			}
		}
	}
};

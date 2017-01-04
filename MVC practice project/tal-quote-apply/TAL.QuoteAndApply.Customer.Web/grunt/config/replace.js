module.exports = function(grunt, options) {

	var brand = options.brandSettings.brand;
	var regex = new RegExp('templateUrl[ \t]*:([ \t]+)\'(\/client\/)(?!' + brand + '\/)', 'g');
	var regexHtmlText = new RegExp('htmlText[ \t]*:([ \t]+)\'(\/client\/)(?!' + brand + '\/)', 'g');
	var ngincludeRegex1 = new RegExp('<ng-include src="\'(\/client\/)(?!' + brand + '\/)', 'g');
	var ngincludeRegex2 = new RegExp('ng-include[ ]?=[ ]?"\'(\/client\/)(?!' + brand + '\/)', 'g');

	console.log('Replacement loaded');

	return {
		dist: {
		  options: {
		    patterns: [
		      {
				match: regex,
              	replacement: function(match, p1, p2, offset, string) {
              		return 'templateUrl:' + p1 + '\'' + p2 + brand + '/';	
              	} 
		      },
		      {
				match: regexHtmlText,
              	replacement: function(match, p1, p2, offset, string) {
              		return 'htmlText:' + p1 + '\'' + p2 + brand + '/';	
              	} 
		      }
		    ]
		  },
		  files: [
		    {expand: true, flatten: false, src: ['client/**/*.js'], dest: './'}
		  ]
		},
		nginclude: {
		  options: {
		    patterns: [
		      {
				match: ngincludeRegex1,
              	replacement: function(match, p1, offset, string) {
              		return '<ng-include src="\'' + p1 + brand + '/';	
              	} 
		      },
		      {
				match: ngincludeRegex2,
              	replacement: function(match, p1, offset, string) {
              		return 'ng-include="\'' + p1 + brand + '/';	
              	} 
		      }
		    ]
		  },
		  files: [
		    {expand: true, flatten: false, src: ['client/**/*.html'], dest: './'}
		  ]
		}
	}
};
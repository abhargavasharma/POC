(function(module) {
	'use strict';

	/**
	 * A service which changes the CSS theme of the site, by replacing the stylesheet link tags in <head>
	 *
	 * @return     {Object}  { description_of_the_return_value }
	 */
	function themeFactory($rootScope) {
		var themes = [
				'tal',
				'yellowBrand'
			],
			currentTheme = 'tal';

		/**
		 * Replaces existing app stylesheet links with new ones
		 * which point to the stylesheets for the given brand/theme.
		 *
		 * @param      {string}  theme   The theme name to switch to.
		 */
		function updateStyleTags(theme) {
			var headNode = document.getElementsByTagName('head')[0],
				stylesheetNodes = headNode.getElementsByTagName('link');

			angular.forEach(stylesheetNodes, function(stylesheet) {
				if (!stylesheet || !stylesheet.hasAttribute('href')) { return; }

				var stylesheetPath = stylesheet.getAttribute('href'),
					newPath = stylesheetPath.replace(/client.*\/assets/, 'client/' + theme + '/assets' );

				stylesheet.setAttribute('href', newPath);
			});

			currentTheme = theme;
		}

		/**
		 * Sets the theme, if it exists
		 *
		 * @param      {<type>}  theme   The theme to switch to.
		 */
		function setTheme(theme) {
			// There must be a theme, and the default is the tal theme
			theme = theme || 'base';

			if (themes.indexOf(theme) !== -1 && theme !== currentTheme) {
				updateStyleTags(theme);
				console.log('New theme is: ', theme);
			}
		}

		function getTheme() {
			return currentTheme;
		}

		function getAllThemes() {
			return themes;
		}

		/**
		 * On state change, update the theme if the query string says to do so.
		 * Otherwise, don't change the theme.
		 */
		$rootScope.$on('$stateChangeStart', function(event, toState, toParams) {
			var newTheme = toParams.brand || currentTheme;
			setTheme(newTheme);
		});

	 	return {
	 		setTheme: setTheme,
	 		getTheme: getTheme,
	 		getAllThemes: getAllThemes
	 	};
	}

	themeFactory.$inject = ['$rootScope'];

	module.factory('styleguideTheme', themeFactory);

}(angular.module('styleguide')));
module.exports = {
	html: '<%= baseSettings.srcIndexFile %>',
	//concat: '{<%= buildAppName %>,<%= baseSettings.ngBaseAppName %>}/**/*{.css,.js}',
	options: {
		dest: '<%= baseSettings.distFolder %>',
		root: './'
	}
};

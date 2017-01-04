var tests = require('../helpers/testHelpers');

var modules = {
  'img' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector , 'alt');

  },
  'label' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector , 'for');

    tests.testHasMatchingFor(moduleSelector);
  },
  '.main-header' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector , 'role');

    tests.testHasRoleEqualTo(moduleSelector, 'banner');
  },
  '.main-nav' : function(moduleSelector){
    tests.testHasAttribute(moduleSelector , 'role');

    tests.testHasRoleEqualTo(moduleSelector, 'nav');
  }
};


module.exports = modules;
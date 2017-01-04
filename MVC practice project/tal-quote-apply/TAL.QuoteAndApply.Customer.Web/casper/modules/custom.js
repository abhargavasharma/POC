var tests = require('../helpers/testHelpers');

var modules = {
  'form-radio' : function(moduleSelector){

    var arrayClasses = ['tal-form-radio', 'tal-form-radio-button', 'tal-form-radio-box'];
    
    tests.testHasOneOfThoseClass(moduleSelector, arrayClasses);

    tests.testHasAttribute(moduleSelector, 'ng-model');
    
    tests.testHasAttributeNotMatching(moduleSelector, 'value', '{{[a-z.]+}}');

  },
  'tal-progress-bar' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector, 'progress');

  },
  'tal-form-switch' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector, 'ng-model');
    
  },
  'tal-tabs' : function(moduleSelector){

    tests.testHasChildren(moduleSelector, 'tal-tab-pane');

    tests.testHasAttribute('tal-tab-pane', 'title');
   
  },
  'tal-figure' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector, 'value');

    tests.testHasAttribute(moduleSelector, 'type');

  },
  'form-checkbox' : function(moduleSelector){

    var arrayClasses = ['tal-form-checkbox', 'tal-form-checkbox-dashed', 'tal-form-checkbox-boxed'];
    
    tests.testHasOneOfThoseClass(moduleSelector, arrayClasses);

    tests.testHasAttribute(moduleSelector, 'ng-model');

    tests.testHasAttribute(moduleSelector, 'tal-name');

    tests.testHasAttribute(moduleSelector, 'tal-id');
  },
  'tal-figure-simple' : function(moduleSelector){
    
    tests.testHasAttribute(moduleSelector, 'value');
  },

  'form-select' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector, 'tal-name');

    tests.testHasAttribute(moduleSelector, 'tal-id');

    tests.testHasAttribute(moduleSelector, 'form-select-items');

    tests.testHasAttribute(moduleSelector, 'ng-model');

  },
  'form-select-textfield' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector, 'tal-name');

    tests.testHasAttribute(moduleSelector, 'tal-id');
    
    tests.testHasAttribute(moduleSelector, 'form-select-items');

    tests.testHasAttribute(moduleSelector, 'ng-model');
     
  },
  '.form-grid' : function(moduleSelector){

    tests.testHasChildren(moduleSelector, '.tal-col');

    tests.testHasChildren(moduleSelector, '.form-group');

  },
  '.basic-info-form' : function(moduleSelector){

    tests.testHasClass(moduleSelector + ' .tal-card', 'tal-review-card');

    tests.testHasOnlyOneChild(moduleSelector, 'tal-card-body');

    tests.testHasAttribute(moduleSelector + ' tal-card-body', 'tal-cq');

    tests.testHasChildren(moduleSelector, ' .form-group');
    
    tests.testHasChildren(moduleSelector + ' .form-group', '.tal-col');

  },
  'tal-documentation' : function(moduleSelector){
    tests.testHasAttribute(moduleSelector, 'index');
  },
  '.tal-form-compact' : function(moduleSelector){
    tests.testHasOnlyOneChild(moduleSelector, 'label.tal-form-compact-label');
  },
  '.tal-form-amount' : function(moduleSelector){

    tests.testHasOnlyOneChild(moduleSelector, 'label.tal-form-compact-label');

    tests.testHasClass(moduleSelector , 'tal-form-compact');

    tests.testHasAttribute(moduleSelector + ' label.tal-form-compact-label', 'for');

    tests.testHasMatchingFor(moduleSelector + ' label.tal-form-compact-label');
  },
  '[type="tel"]' : function(moduleSelector){

    tests.testHasAttribute(moduleSelector , 'ui-mask');
  }
};


module.exports = modules;
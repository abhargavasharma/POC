var helpers = require('../helpers/domHelpers');
var _ = require('underscore');

var forEachInstanceOf = function(selector, callback){
  var arrayElInfo = helpers.getElementsInfo(selector);
  casper.each(arrayElInfo, function(casper, elInfo, j){
    callback(casper, elInfo, j);
  });
}

var tests = {

  // Checks if an element has an attribute by looking at the casper element info object.
  hasAttribute : function(elInfo, attributeName){
    var attribute = helpers.getAttribute(elInfo, attributeName);
    return  typeof attribute === 'string' && attribute.length > 0
  },

  // Checks if an element has an empty attribute by looking at the casper element info object.
  // @TODO: attribute.length === '' seems wrong, may been to be corrected.
  hasDirective : function(elInfo, attributeName){
    var attribute = helpers.getAttribute(elInfo, attributeName);
    return  typeof attribute === 'string' && attribute.length === ''
  },

  // Checks if an element has a class by looking at the casper element  info
  hasClass : function(elInfo, className){
    var classes = helpers.getClass(elInfo);
    return classes.length > 0 && classes.indexOf(className) != -1;
  },

  // Checks if an element has a class by looking at the casper element  info
  hasOneOfClass : function(elInfo, classList){
    var classes = helpers.getClass(elInfo);
    return classes.length > 0 && _.intersection(classes, classList).length > 0;
  },
  hasAttributeNotMatching : function(elInfo, attributeName, regularExpression){
    var pattern = new RegExp(regularExpression);
    var attribute = helpers.getAttribute(elInfo, attributeName);
    return !pattern.test(attribute);
  },
  hasAttributeMatching : function(elInfo, attributeName, regularExpression){
    var pattern = new RegExp(regularExpression);
    var attribute = helpers.getAttribute(elInfo, attributeName);
    return pattern.test(attribute);
  },
  // Checks if an element has children matching a selector, using jquery in an casper evaluate
  hasChildren : function(selector, childSelector){
    return casper.evaluate(function(selector, childSelector) {
      var response = true;
      $(selector).each(function(i, el){
        if($(el).find(childSelector).length === 0){
          response = false;
        }
      });
      return response;
    }, selector, childSelector);
  },
  hasRoleValue : function(elInfo, value){
    var attribute = helpers.getAttribute(elInfo, 'role');
    return  typeof attribute === 'string' && attribute  === value;
  },
  hasMatchingFor : function(selector){
    return casper.evaluate(function(selector) {
      var response = true;
      $(selector).not('.label-radio').each(function(i, el){
        var forValue = $(el).attr('for');
        if($("[id='"+forValue+"']").length === 0 && $("[tal-id='"+forValue+"']").length === 0 && $("[tal-form-id='"+forValue+"']").length === 0){
          response = false
        }
      });
      return response;
    }, selector);
  },
  // Checks if an element has only one child matching a selector, using jquery in an casper evaluate
  hasOnlyOneChild : function(selector, childSelector){
    return casper.evaluate(function(selector, childSelector) {
      var response = true;
      $(selector).each(function(i, el){
        if($(el).find(childSelector).length != 1){
          console.log('hasOnlyOneChild',selector, childSelector, $(el).find(childSelector).html() );
          response = false
        }
      });
      return response;
    }, selector, childSelector);
  },
  testHasChildren : function(selector, childSelector){
    casper.test.assert(
      tests.hasChildren(selector, childSelector), 
      '"' + selector + '" has ' + childSelector
    );
  },
  testHasAttribute : function(selector, attributeName){
    forEachInstanceOf(selector, function(casper, elInfo, j){
      casper.test.assert(
        tests.hasAttribute(elInfo, attributeName), 
        '"' + selector + '" has an attribute '+attributeName
      );
    });
  },
  testHasAttributeNotMatching : function(selector, attributeName, regularExpression){
    forEachInstanceOf(selector, function(casper, elInfo, j){
      casper.test.assert(
        tests.hasAttributeNotMatching(elInfo, attributeName, regularExpression), 
        '"' + selector + '" has an attribute '+attributeName+' not matching' + regularExpression
      );
    });
  },
  testHasAttributeMatching : function(selector, attributeName, regularExpression){
    forEachInstanceOf(selector, function(casper, elInfo, j){
      casper.test.assert(
        tests.hasAttributeMatching(elInfo, attributeName, regularExpression), 
        '"' + selector + '" has an attribute '+attributeName+' not matching' + regularExpression
      );
    });
  },
  testHasOnlyOneChild : function(selector, childSelector){
    casper.test.assert(
      tests.hasOnlyOneChild(selector, childSelector), 
      '"' + selector + '" has only one ' + childSelector
    );
  },
  testHasOneOfThoseClass : function(selector, classList){
    forEachInstanceOf(selector, function(casper, elInfo, j){
      casper.test.assert(
        tests.hasOneOfClass(elInfo, classList), 
        '"' + selector + '" has either class ' + classList.join(',')
      );
    });
  },
  testHasClass : function(selector, className){
    forEachInstanceOf(selector, function(casper, elInfo, j){
      casper.test.assert(
        tests.hasClass(elInfo, className), 
        '"' + selector + '" has class ' + className
      );
    });
  },
  testHasRoleEqualTo : function(selector, roleValue){
    forEachInstanceOf(selector, function(casper, elInfo, j){
      casper.test.assert(
        tests.hasRoleValue(elInfo, roleValue), 
        '"' + selector + '" has a role equal to ' + roleValue
      );
    });
    
  },
  testHasMatchingFor : function(selector){
    
    casper.test.assert(
      tests.hasMatchingFor(selector), 
      '"' + selector + '" has matching id for a for '
    );
  }
};
module.exports = tests;
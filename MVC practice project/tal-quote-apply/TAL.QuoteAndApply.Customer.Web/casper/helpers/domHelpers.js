var _ = require('underscore');

var helpers = {
  getClass : function(elInfo){
    return elInfo.attributes["class"].split(' ');
  },
  getAttribute : function (elInfo, attributeName){
    return elInfo.attributes[attributeName];
  },
  getElementsInfo : function(selector){
    return casper.getElementsInfo(selector);
  },
  dump: function(obj){
    return JSON.stringify(obj);
  }
}

module.exports = helpers;
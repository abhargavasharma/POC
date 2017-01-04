// Module checkers
// Checks all your modules
// =====================
var accessibility = require('../modules/accessibility');
var custom = require('../modules/custom');
var tests = require('../helpers/testHelpers');
var _ = require('underscore');

var modules = _.extend({}, accessibility, custom);


module.exports = modules;
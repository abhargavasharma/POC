(function (module) {
    'use strict';

    function talOccupationSearchService($resource) {

        var searchResource = $resource('/api/search/question/occupation',
            {
                query: '@query',
                limit: '@limit'
            },
            {
                get: {
                    method: 'GET',
                    isArray: true
                }
            }
        );

        /*jshint validthis: true */
        this.search = function (query, onSuccess) {
            var params = {
                query: query,
                limit: 30
            };

            searchResource.get(params, onSuccess);
        };
    }

    module.service('talOccupationSearchService', talOccupationSearchService);

    talOccupationSearchService.$inject = ['$resource'];

})(angular.module('appCustomerPortal'));

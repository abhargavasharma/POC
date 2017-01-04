using System;
using System.Collections.Generic;
using Rally.RestApi;
using Rally.RestApi.Response;

namespace Rally.ServiceLayer.Rally
{
    public interface IRallyUserStoryNotesUpdate
    {
        void UpdateStory(string defectId, string messageToAdd);
    }

    public class RallyUserStoryNotesUpdate : IRallyUserStoryNotesUpdate
    {
        private readonly IRallyApiKeyProvider _rallyApiKeyProvider;

        public RallyUserStoryNotesUpdate(IRallyApiKeyProvider rallyApiKeyProvider)
        {
            _rallyApiKeyProvider = rallyApiKeyProvider;
        }

        public RallyUserStoryNotesUpdate()
            : this(new RallyApiKeyProvider())
        {

        }

        public void UpdateStory(string defectId, string messageToAdd)
        {
            var restApi = new RallyRestApi();
            var token = _rallyApiKeyProvider.GetToken();
            restApi.AuthenticateWithApiKey(token, "https://rally1.rallydev.com", null);

            var request = new Request("HierarchicalRequirement");
            request.Fetch = new List<string>() { "FormattedID", "Notes" };
            request.Query = new Query("FormattedID", Query.Operator.Equals, defectId);
            QueryResult queryResult = restApi.Query(request);
            foreach (var result in queryResult.Results)
            {
                result["Notes"] = result["Notes"] + Environment.NewLine + messageToAdd;
                restApi.Update(result["_ref"], result);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Rally.RestApi;
using Rally.RestApi.Response;

namespace Rally.ServiceLayer.Rally
{
    public interface IRallyDefectNotesUpdate
    {
        void UpdateDefect(string defectId, string messageToAdd);
    }

    public class RallyDefectNotesUpdate : IRallyDefectNotesUpdate
    {
        private readonly IRallyApiKeyProvider _rallyApiKeyProvider;

        public RallyDefectNotesUpdate(IRallyApiKeyProvider rallyApiKeyProvider)
        {
            _rallyApiKeyProvider = rallyApiKeyProvider;
        }

        public RallyDefectNotesUpdate()
            : this(new RallyApiKeyProvider())
        {
            
        }

        public void UpdateDefect(string defectId, string messageToAdd)
        {
            var restApi = new RallyRestApi();
            var token = _rallyApiKeyProvider.GetToken();
            restApi.AuthenticateWithApiKey(token, "https://rally1.rallydev.com", null);

            var request = new Request("Defect");
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
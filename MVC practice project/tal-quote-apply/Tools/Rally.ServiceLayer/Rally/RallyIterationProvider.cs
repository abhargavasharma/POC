using System.Linq;
using Rally.RestApi;
using Rally.RestApi.Response;

namespace Rally.ServiceLayer.Rally
{
    public interface IRallyIterationProvider
    {
        string GetCurrentIteration(string projectUniqueId);
    }

    public class RallyIterationProvider : IRallyIterationProvider
    {
        private readonly IRallyApiKeyProvider _rallyApiKeyProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RallyIterationProvider(IRallyApiKeyProvider rallyApiKeyProvider, IDateTimeProvider dateTimeProvider)
        {
            _rallyApiKeyProvider = rallyApiKeyProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public RallyIterationProvider()
            : this(new RallyApiKeyProvider(), new DateTimeProvider())
        {
        }

        public string GetCurrentIteration(string projectUniqueId)
        {
            var restApi = new RallyRestApi();
            var token = _rallyApiKeyProvider.GetToken();
            restApi.AuthenticateWithApiKey(token, "https://rally1.rallydev.com", null);

            var dateNow = _dateTimeProvider.GetCurrentDateTime();
            var request = new Request("Iteration");
            request.Query =
                new Query("StartDate", Query.Operator.LessThanOrEqualTo, dateNow.ToString("yyyy-MM-ddThh:mm:ss")).And(
                    new Query("EndDate", Query.Operator.GreaterThanOrEqualTo, dateNow.ToString("yyyy-MM-ddThh:mm:ss")));

            QueryResult queryResult = restApi.Query(request);
            var currentIteration = queryResult.Results.FirstOrDefault(r => r["Project"]["_refObjectName"].ToString().Equals(projectUniqueId));
            return currentIteration?["_refObjectUUID"];
        }
    }
}

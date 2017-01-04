using System.Collections.Generic;
using System.Linq;
using Rally.RestApi;
using Rally.RestApi.Response;

namespace Rally.ServiceLayer.Rally
{
    public class UserStory
    {
        public UserStory(string friendlyId, string name)
        {
            FriendlyId = friendlyId;
            Name = name;
        }

        public string FriendlyId { get; private set; }
        public string Name { get; private set; }
    }

    public class Defect
    {
        public Defect(string friendlyId, string name)
        {
            FriendlyId = friendlyId;
            Name = name;
        }

        public string FriendlyId { get; private set; }
        public string Name { get; private set; }
    }


    public interface IRallyUserStoryProvider
    {
        UserStory[] GetUserStoriesForIteration(string iterationId, string kanbanState);
        Defect[] GetDefectsForIteration(string iterationId, string kanbanState);
        IEnumerable<UserStory> GetUserStories(IEnumerable<string> userStories);
        IEnumerable<Defect> GetDefects(IEnumerable<string> defects);
    }

    public class RallyUserStoryProvider : IRallyUserStoryProvider
    {
        private readonly IRallyApiKeyProvider _rallyApiKeyProvider;

        public RallyUserStoryProvider(IRallyApiKeyProvider rallyApiKeyProvider)
        {
            _rallyApiKeyProvider = rallyApiKeyProvider;
        }

        public RallyUserStoryProvider()
            : this(new RallyApiKeyProvider())
        {
            
        }

        public IEnumerable<UserStory> GetUserStories(IEnumerable<string> userStories )
        {
            var restApi = new RallyRestApi();
            var token = _rallyApiKeyProvider.GetToken();
            restApi.AuthenticateWithApiKey(token, "https://rally1.rallydev.com", null);

            foreach (var userStory in userStories)
            {
                var request = new Request("HierarchicalRequirement");
                request.Fetch = new List<string>() { "FormattedID", "Name" };
                request.Query = new Query("FormattedID", Query.Operator.Equals, userStory);

                QueryResult queryResult = restApi.Query(request);
                var results = queryResult.Results.Select(r => new UserStory(r["FormattedID"], r["Name"])).ToArray();
                foreach (var result in results)
                {
                    yield return result;
                }
            }           
        }

        public IEnumerable<Defect> GetDefects(IEnumerable<string> defects)
        {
            var restApi = new RallyRestApi();
            var token = _rallyApiKeyProvider.GetToken();
            restApi.AuthenticateWithApiKey(token, "https://rally1.rallydev.com", null);

            foreach (var defect in defects)
            {
                var request = new Request("Defect");
                request.Fetch = new List<string>() { "FormattedID", "Name" };
                request.Query = new Query("FormattedID", Query.Operator.Equals, defect);

                QueryResult queryResult = restApi.Query(request);
                var results = queryResult.Results.Select(r => new Defect(r["FormattedID"], r["Name"])).ToArray();
                foreach (var result in results)
                {
                    yield return result;
                }
            }
        }

        public UserStory[] GetUserStoriesForIteration(string iterationId, string kanbanState)
        {
            var restApi = new RallyRestApi();
            var token = _rallyApiKeyProvider.GetToken();
            restApi.AuthenticateWithApiKey(token, "https://rally1.rallydev.com", null);

            var request = new Request("HierarchicalRequirement");
            request.Fetch = new List<string>() { "FormattedID", "Name" };
            request.Query =
                new Query("Iteration", Query.Operator.DoesNotEqual, null)
                    .And(new Query("Iteration.ObjectUUID", Query.Operator.Equals, iterationId))
                    .And(new Query("c_KanbanState", Query.Operator.Equals, kanbanState))
                    .And(new Query("Ready", Query.Operator.Equals, true.ToString()));

            QueryResult queryResult = restApi.Query(request);
            return queryResult.Results.Select(r => new UserStory(r["FormattedID"], r["Name"])).ToArray();
        }

        public Defect[] GetDefectsForIteration(string iterationId, string kanbanState)
        {
            var restApi = new RallyRestApi();
            var token = _rallyApiKeyProvider.GetToken();
            restApi.AuthenticateWithApiKey(token, "https://rally1.rallydev.com", null);

            var request = new Request("Defect");
            request.Fetch = new List<string>() { "FormattedID", "Name" };
            request.Query =
                new Query("Iteration", Query.Operator.DoesNotEqual, null)
                    .And(new Query("Iteration.ObjectUUID", Query.Operator.Equals, iterationId))
                    .And(new Query("c_KanbanState", Query.Operator.Equals, kanbanState))
                    .And(new Query("Ready", Query.Operator.Equals, true.ToString()));

            QueryResult queryResult = restApi.Query(request);
            return queryResult.Results.Select(r => new Defect(r["FormattedID"], r["Name"])).ToArray();
        }
    }
}
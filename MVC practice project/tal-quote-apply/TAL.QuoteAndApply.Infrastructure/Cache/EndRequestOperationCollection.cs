using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TAL.QuoteAndApply.Infrastructure.Http;

namespace TAL.QuoteAndApply.Infrastructure.Cache
{
    public interface IEndRequestOperationCollection
    {
        void AddOrUpdateAction(string key, Action action);
        void ExecuteTasks();
    }

    public class EndRequestOperationCollection : IEndRequestOperationCollection
    {
        private const string ContextLookupKey = "TAL.EndRequestOperationCollection";

        private readonly IHttpContextProvider _httpContextProvider;

        private ConcurrentDictionary<string, Action> Actions
        {
            get
            {
                var context = _httpContextProvider.GetCurrentContext();
                context.Items[ContextLookupKey] = context.Items[ContextLookupKey] ?? new ConcurrentDictionary<string, Action>();
                return context.Items[ContextLookupKey] as ConcurrentDictionary<string, Action>;
            }
        }

        public EndRequestOperationCollection(IHttpContextProvider httpContextProvider)
        {
            _httpContextProvider = httpContextProvider;
        }

        public void AddOrUpdateAction(string key, Action action)
        {
            Actions.AddOrUpdate(key, action, (k, v) => action);
        }

        public void ExecuteTasks()
        {
            foreach (var action in Actions.Values)
            {
                action.Invoke();
            }
            Actions.Clear();
        }
    }
}
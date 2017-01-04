using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace TAL.QuoteAndApply.Search.Models
{
    public class DataItem<T>
    {
        public DataItem()
        {
        }

        public DataItem(IEnumerable<T> items, string key)
            : this()
        {
            Id = ObjectId.GenerateNewId();
            Items = items.ToList();
            IndexKey = key;
        }

        public ObjectId Id { get; set; }
        public string IndexKey { get; set; }
        public List<T> Items { get; set; } 
    }
}
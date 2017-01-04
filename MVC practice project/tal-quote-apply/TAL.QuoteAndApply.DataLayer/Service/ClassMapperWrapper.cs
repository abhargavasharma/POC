using System;
using System.Linq.Expressions;
using TAL.QuoteAndApply.DataLayer.Service.Dapper;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.DataLayer.Service
{
    public class ClassMapperWrapper<T> where T : DbItem
    {
        private readonly CustomClassMapper<T> _classMapper;

        public ClassMapperWrapper(CustomClassMapper<T> classMapper)
        {
            _classMapper = classMapper;
        }

        public void MapProperty(Expression<Func<T, object>> expression, string columnName)
        {
            _classMapper.Map(expression).Column(columnName);
        }

        public void MakePropertyReadOnly(Expression<Func<T, object>> expression)
        {
            _classMapper.Map(expression).ReadOnly();
        }

        public void MapTable(string tableName)
        {
            _classMapper.Table(tableName);
        }

        public void MapSchema(string schema)
        {
            _classMapper.Schema(schema);
        }

        public void Ignore(Expression<Func<T, object>> expression)
        {
            _classMapper.Map(expression).Ignore();
        }

        public void Encrypt(Expression<Func<T, object>> expression)
        {
            _classMapper.Map(expression).Encrypt();
        }
    }
}
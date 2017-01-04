using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using DapperExtensions;
using TAL.QuoteAndApply.DataLayer.Contract;
using TAL.QuoteAndApply.DataLayer.Exceptions;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;

namespace TAL.QuoteAndApply.DataLayer.Repository
{
    public enum Op
    {
        Eq,
        Gt,
        Ge,
        Lt,
        Le,
        StartsWith,
    }


    public abstract class BaseRepository<T> : ICrudDbItemRepository<T>, IBulkCrudDbItemRepository<T>
            where T : DbItem
    {
        private readonly string _connectionString;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IDataLayerExceptionFactory _dataLayerExceptionFactory;
        private readonly IDbItemEncryptionService _dbItemEncryptionService;

        protected BaseRepository(string connectionString, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
        {
            _connectionString = connectionString;
            _currentUserProvider = currentUserProvider;
            _dataLayerExceptionFactory = dataLayerExceptionFactory;
            _dbItemEncryptionService = dbItemEncryptionService;
        }

        public int Execute(string sql, object param = null, bool useTransaction = false, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                IDbTransaction transaction = null;
                if (useTransaction)
                {
                    transaction = connection.BeginTransaction();
                }

                try
                {
                    var retVal = connection.Execute(sql, param, transaction, commandTimeout, commandType);

                    if (useTransaction)
                    {
                        transaction.Commit();
                    }
                    return retVal;
                }
                catch (Exception e)
                {
                    if (useTransaction)
                    {
                        transaction.Rollback();
                    }

                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        //TODO: Need a better way to query. LINQ or only pass in where clause
        public IEnumerable<T> Query(string sql, object param = null, bool useTransaction = false, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                IDbTransaction transaction = null;
                if (useTransaction)
                {
                    transaction = connection.BeginTransaction();
                }
                try
                {
                    var retVal = connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);

                    if (useTransaction)
                    {
                        transaction.Commit();
                    }

                    return retVal.Select(_dbItemEncryptionService.Decrypt);
                }
                catch (Exception e)
                {
                    if (useTransaction)
                    {
                        transaction.Rollback();
                    }

                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        public T Get(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    return _dbItemEncryptionService.Decrypt(connection.Get<T>(id));
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        public T Insert(T dbItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SetCreatedAttributes(dbItem);

                connection.Open();
                try
                {
                    _dbItemEncryptionService.Encrypt(dbItem); // encrypt, save then decrypt model

                    var dyn = connection.Insert<T>(dbItem);
                    return dbItem = Get(dyn);
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
                finally
                {
                    _dbItemEncryptionService.Decrypt(dbItem);
                }
            }
        }

        public void Update(T dbItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SetModifiedAttributes(dbItem);
                connection.Open();

                try
                {
                    _dbItemEncryptionService.Encrypt(dbItem); // encrypt, save then decrypt model

                    if (!connection.Update<T>(dbItem))
                    {
                        
                        throw new DataLayerUpdateFailedException(
                            $"Update failed. An item with the primary key {dbItem.Id} and Row Version {Convert.ToBase64String(dbItem.RV)} could not be found for {GetType().Name}.",
                            null);
                    }
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
                finally
                {
                    _dbItemEncryptionService.Decrypt(dbItem);
                }
            }
        }

        public bool TryUpdate(T dbItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SetModifiedAttributes(dbItem);
                connection.Open();

                try
                {
                    _dbItemEncryptionService.Encrypt(dbItem); // encrypt, save then decrypt model

                    return connection.Update<T>(dbItem);
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
                finally
                {
                    _dbItemEncryptionService.Decrypt(dbItem);
                }
            }
        }

        public bool Delete(T dbItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    return connection.Delete<T>(dbItem);
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        public void Insert(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();

                try
                {
                    foreach (var item in items)
                    {
                        SetCreatedAttributes(item);
                        _dbItemEncryptionService.Encrypt(item);
                        connection.Insert(item, trans);
                    }

                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
                finally
                {
                    foreach (var item in items)
                    {
                        _dbItemEncryptionService.Decrypt(item);
                    }
                }
            }
        }

        public IEnumerable<bool> Update(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();

                try
                {
                    var results = new List<bool>();

                    foreach (var item in items)
                    {
                        SetModifiedAttributes(item);
                        _dbItemEncryptionService.Encrypt(item);
                        results.Add(connection.Update(item, trans));
                    }

                    trans.Commit();

                    return results;
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
                finally
                {
                    foreach (var item in items)
                    {
                        _dbItemEncryptionService.Decrypt(item);
                    }
                }
            }
        }

        protected IEnumerable<T> Where(Expression<Func<T, object>> property, Op operation, object value, bool not = false)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var predicateValue = value;
                if (operation == Op.StartsWith)
                {
                    predicateValue = StartsWithValue(value);
                }

                var dapperOperator = GetDapperOperator(operation);
                var predicate = Predicates.Field<T>(property, dapperOperator, predicateValue, not);
                connection.Open();

                try
                {
                    return connection.GetList<T>(predicate).Select(_dbItemEncryptionService.Decrypt);
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        protected IEnumerable<T> Query(IDbItemPredicate<T> predicate, int? limit = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var predicateGroup = predicate as PredicateAndOr<T>;
                if (predicateGroup == null)
                {
                    throw new ApplicationException("IPredicate<T> predicate was not of type PredicateAndOr<T>");
                }

                try
                {
                    if (!limit.HasValue)
                    {
                        return connection.GetList<T>(predicateGroup.PredicateGroup)
                            .Select(_dbItemEncryptionService.Decrypt);
                    }
                    return connection.GetLimitedList<T>(limit.Value, predicateGroup.PredicateGroup)
                        .Select(_dbItemEncryptionService.Decrypt);
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        public IEnumerable<T> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                try
                {
                    return connection.GetList<T>().Select(_dbItemEncryptionService.Decrypt);
                }
                catch (Exception e)
                {
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        public IEnumerable<bool> Delete(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();

                try
                {
                    var results = new List<bool>();

                    foreach (var item in items)
                    {
                        results.Add(connection.Delete(item, trans));
                    }

                    trans.Commit();

                    return results;
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        public bool Delete(IDbItemPredicate<T> predicate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();

                var predicateGroup = predicate as PredicateAndOr<T>;
                if (predicateGroup == null)
                {
                    throw new ApplicationException("IPredicate<T> predicate was not of type PredicateAndOr<T>");
                }

                try
                {
                    var result = connection.Delete<T>(predicateGroup.PredicateGroup, trans);
                    trans.Commit();

                    return result;
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw _dataLayerExceptionFactory.CreateFrom(e);
                }
            }
        }

        private void SetCreatedAttributes(DbItem entity)
        {
            var currentUser = _currentUserProvider.GetForApplication();

            entity.CreatedTS = DateTime.Now;
            entity.CreatedBy = currentUser.UserName;
            SetModifiedAttributes(entity);
        }

        private void SetModifiedAttributes(DbItem entity)
        {
            var currentUser = _currentUserProvider.GetForApplication();

            entity.ModifiedTS = DateTime.Now;
            entity.ModifiedBy = currentUser.UserName;
        }

        private Operator GetDapperOperator(Op from)
        {
            switch (from)
            {
                case Op.Eq:
                    return Operator.Eq;
                case Op.Ge:
                    return Operator.Ge;
                case Op.Gt:
                    return Operator.Gt;
                case Op.Le:
                    return Operator.Le;
                case Op.Lt:
                    return Operator.Lt;
                case Op.StartsWith:
                default:
                    return Operator.Like;
            }
        }

        private string StartsWithValue(object value)
        {
            var strValue = value.ToString();
            return strValue + "%";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using DapperExtensions;
using DapperExtensions.Mapper;

namespace TAL.QuoteAndApply.DataLayer.Service.Dapper
{
    /// <summary>
    /// This is a copy of the class mapper from dapper extensions, with an added inclusion to support mapping
    /// from a custom global map registry
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomClassMapper<T> : IClassMapper<T> where T : class
    {
        /// <summary>
        /// Gets or sets the schema to use when referring to the corresponding table name in the database.
        /// </summary>
        public string SchemaName { get; protected set; }

        /// <summary>
        /// Gets or sets the table to use in the database.
        /// </summary>
        public string TableName { get; protected set; }

        /// <summary>
        /// A collection of properties that will map to columns in the database table.
        /// </summary>
        public IList<IPropertyMap> Properties { get; private set; }

        public Type EntityType
        {
            get { return typeof(T); }
        }

        public CustomClassMapper()
        {
            PropertyTypeKeyTypeMapping = new Dictionary<Type, KeyType>
            {
                { typeof(byte), KeyType.Identity }, { typeof(byte?), KeyType.Identity },
                { typeof(sbyte), KeyType.Identity }, { typeof(sbyte?), KeyType.Identity },
                { typeof(short), KeyType.Identity }, { typeof(short?), KeyType.Identity },
                { typeof(ushort), KeyType.Identity }, { typeof(ushort?), KeyType.Identity },
                { typeof(int), KeyType.Identity }, { typeof(int?), KeyType.Identity },
                { typeof(uint), KeyType.Identity}, { typeof(uint?), KeyType.Identity },
                { typeof(long), KeyType.Identity }, { typeof(long?), KeyType.Identity },
                { typeof(ulong), KeyType.Identity }, { typeof(ulong?), KeyType.Identity },
                { typeof(BigInteger), KeyType.Identity }, { typeof(BigInteger?), KeyType.Identity },
                { typeof(Guid), KeyType.Guid }, { typeof(Guid?), KeyType.Guid },
            };

            Properties = new List<IPropertyMap>();
            Table(typeof(T).Name);
        }

        protected Dictionary<Type, KeyType> PropertyTypeKeyTypeMapping { get; private set; }

        public void Schema(string schemaName)
        {
            SchemaName = schemaName;
        }

        public void Table(string tableName)
        {
            TableName = tableName;
        }

        public void AutoMap()
        {
            AutoMap(null);
        }

        public void AutoMap(Func<Type, PropertyInfo, bool> canMap)
        {
            Type type = typeof(T);
            bool hasDefinedKey = Properties.Any(p => p.KeyType != KeyType.NotAKey);
            PropertyMap keyMap = null;
            foreach (var propertyInfo in type.GetProperties())
            {
                if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                if ((canMap != null && !canMap(type, propertyInfo)))
                {
                    continue;
                }

                PropertyMap map = Map(propertyInfo);
                if (!hasDefinedKey)
                {
                    if (string.Equals(map.PropertyInfo.Name, "id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        keyMap = map;
                    }

                    if (keyMap == null && map.PropertyInfo.Name.EndsWith("id", true, CultureInfo.InvariantCulture))
                    {
                        keyMap = map;
                    }
                }
            }

            if (keyMap != null)
            {
                keyMap.Key(PropertyTypeKeyTypeMapping.ContainsKey(keyMap.PropertyInfo.PropertyType)
                    ? PropertyTypeKeyTypeMapping[keyMap.PropertyInfo.PropertyType]
                    : KeyType.Assigned);
            }
        }

        /// <summary>
        /// Fluently, maps an entity property to a column
        /// </summary>
        public PropertyMap Map(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Map(propertyInfo);
        }

        /// <summary>
        /// Fluently, maps an entity property to a column
        /// </summary>
        public PropertyMap Map(PropertyInfo propertyInfo)
        {
            PropertyMap result = new PropertyMap(propertyInfo);
            this.GuardForDuplicatePropertyMap(result);
            Properties.Add(result);
            return result;
        }

        private void GuardForDuplicatePropertyMap(PropertyMap result)
        {
            if (Properties.Any(p => p.Name.Equals(result.Name)))
            {
                throw new ArgumentException(string.Format("Duplicate mapping for property {0} detected.", result.Name));
            }
        }
    }
}
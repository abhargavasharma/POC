using System;
using System.Collections.Generic;

namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public interface IScope
    {
        IAdd InSingletonScope();
    }

    public class SimpleDependencyMapper : ISimpleDependencyMapper, IBinder, IDisposable
    {
        private TypeMapItem _workingItem;
        private List<TypeMapItem> _typeMap;
        public List<TypeMapItem> TypeMaps
        {
            get
            {
                return _typeMap;
            }
        }

        public SimpleDependencyMapper()
        {
            _typeMap = new List<TypeMapItem>();
        }

        public IAdd Add<T>()
        {
            var of = typeof(T);
            _workingItem = new TypeMapItem
            {
                ResolveToType = of,
                WithMatchInterface = true
            };
            _typeMap.Add(_workingItem);
            return this;
        }

        public IBindTo WhenRequesting<T>()
        {
            var of = typeof(T);
            _workingItem = new TypeMapItem
            {
                RegisteredType = of
            };
            _typeMap.Add(_workingItem);
            return this;
        }

        public IBindOf ProvideImplementationOf<T>()
        {
            _workingItem.ResolveToType = typeof(T);
            return this;
        }

        public void Dispose()
        {
            _typeMap = null;
        }

        public IAdd InSingletonScope()
        {
            _workingItem.IsSingletonScope = true;
            return this;
        }
    }
}

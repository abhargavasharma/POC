using System.Collections.Generic;

namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public interface ISimpleDependencyMapper : IBindOf, IAdd
    {
        List<TypeMapItem> TypeMaps { get; }
    }
}
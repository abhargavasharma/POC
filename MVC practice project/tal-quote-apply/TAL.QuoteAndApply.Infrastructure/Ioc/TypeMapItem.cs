using System;

namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public class TypeMapItem
    {
        public Type ResolveToType { get; set; }
        public Type RegisteredType { get; set; }
        public bool WithMatchInterface { get; set; }
        public bool IsSingletonScope { get; set; }
    }
}
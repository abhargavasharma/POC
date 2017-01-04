namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public interface IBindTo
    {
        IBindOf ProvideImplementationOf<T>();
    }
}
namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public interface IBindOf : IScope
    {
        IBindTo WhenRequesting<T>();
    }
}
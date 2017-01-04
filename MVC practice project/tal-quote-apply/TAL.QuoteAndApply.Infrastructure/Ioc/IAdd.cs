namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public interface IAdd : IScope
    {
        IAdd Add<T>();
    }
}
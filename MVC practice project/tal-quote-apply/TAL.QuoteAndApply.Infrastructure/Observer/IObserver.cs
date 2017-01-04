namespace TAL.QuoteAndApply.Infrastructure.Observer
{
    public interface ISimpleObserver<TChange>
    {
        void Update(TChange subjectChange);
    }
}
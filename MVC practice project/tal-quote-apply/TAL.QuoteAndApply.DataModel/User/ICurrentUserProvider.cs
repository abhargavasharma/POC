namespace TAL.QuoteAndApply.DataModel.User
{
    public interface ICurrentUserProvider
    {
        ICurrentUser GetForApplication();
    }
}
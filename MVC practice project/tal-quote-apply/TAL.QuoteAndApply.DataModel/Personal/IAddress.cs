namespace TAL.QuoteAndApply.DataModel.Personal
{
    public interface IAddress
    {
        string Address { get; set; }
        string Suburb { get; set; }
        State State { get; set; }
        string Postcode { get; set; }
        Country Country { get; set; }
    }

    public interface IName
    {
        Title Title { get; set; }
        string FirstName { get; set; }
        string Surname { get; set; }
    }
}

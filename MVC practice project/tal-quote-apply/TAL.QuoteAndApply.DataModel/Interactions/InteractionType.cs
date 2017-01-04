namespace TAL.QuoteAndApply.DataModel.Interactions
{
    
    public enum InteractionType
    {
        //NOTE: When adding enum here, make sure a corresponding entry exists in the InteractionType table in the DB

        Quote_Accessed = 1,
        Pipeline_Status_Change = 2,
        Saved_By_Customer = 3,
        Retrieved_By_Customer = 4,
        Created_By_Agent = 5,
        Created_By_Customer = 6,
        Submitted_To_Policy_Admin_System = 7,
        Referred_To_Underwriter = 8,
        Referral_Completed_By_Underwriter = 9,
        Quote_Save_Email_Sent = 10,
        Policy_Note_Added = 11,
        Customer_Submit_Application_Referred = 12,
		Quote_Email_Sent_From_Sales_Portal = 13,
        Application_Confirmation_Email_Sent = 14,
        Customer_Callback_Requested = 15,
        Customer_Webchat_Requested = 16
    }
}



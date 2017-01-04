using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface IPolicyNoteRequestConverter
    {
        PolicyNoteResult From(PolicyNoteResultViewModel policyNoteResultModel);
        PolicyNoteResultViewModel From(PolicyNoteResult policyNoteResult);
    }

    public class PolicyNoteRequestConverter : IPolicyNoteRequestConverter
    {
        public PolicyNoteResult From(PolicyNoteResultViewModel policyNoteResultModel)
        {
            return new PolicyNoteResult(policyNoteResultModel.Id, policyNoteResultModel.NoteText);
        }

        public PolicyNoteResultViewModel From(PolicyNoteResult policyNoteResult)
        {
            return new PolicyNoteResultViewModel()
            {
                Id = policyNoteResult.Id,
                NoteText = policyNoteResult.NoteText
            };
        }
    }
}
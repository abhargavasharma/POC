using System;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IReferral
    {
        int PolicyId { get; }
        string AssignedTo { get; }
        DateTime? AssignedToTS { get; }
        bool IsCompleted { get; }
        DateTime? CompletedTS { get; }
        DateTime CreatedTS { get; }
        string CreatedBy { get; }
        string CompletedBy { get; }
    }

    public class ReferralDto : DbItem, IReferral
    {
        public int PolicyId { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? AssignedToTS { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedTS { get; set; }
        public string CompletedBy { get; set; }
    }
}

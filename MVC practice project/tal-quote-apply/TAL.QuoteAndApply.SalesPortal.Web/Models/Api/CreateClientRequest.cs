using System;
using System.ComponentModel.DataAnnotations;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class CreateClientRequest
    {
        public CreateClientRequest()
        {            
            PolicyOwner = new ClientRequest();
        }

        [Required]
        public ClientRequest PolicyOwner { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

    }
}
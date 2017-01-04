using System;
using System.Linq;
using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Web.Shared.Session;
using TAL.QuoteAndApply.Web.Shared.Workflow;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    [ApplicationStepValidationFilter]
    public abstract class BaseCustomerViewController : Controller
    {
        protected readonly IBaseCustomerControllerHelper _baseCustomerControllerHelper;

        protected BaseCustomerViewController(IBaseCustomerControllerHelper baseCustomerControllerHelper)
        {
            _baseCustomerControllerHelper = baseCustomerControllerHelper;
            _baseCustomerControllerHelper.SetMainQuoteValues(ViewBag);
        }

        public PolicyOverviewResult PolicyOverview => _baseCustomerControllerHelper.PolicyOverview.Value;
        public string QuoteReferenceNumber => _baseCustomerControllerHelper.QuoteReferenceNumber;

        public void ClearSession()
        {
            _baseCustomerControllerHelper.Session.Clear();
        }
        
    }
}
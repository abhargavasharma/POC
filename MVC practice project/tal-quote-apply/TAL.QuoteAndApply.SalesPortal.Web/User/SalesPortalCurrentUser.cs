using System.Collections.Generic;
using System;
using System.Web.Mvc;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.ServiceLayer.User;

namespace TAL.QuoteAndApply.SalesPortal.Web.User
{
    public class SalesPortalCurrentUser : IApplicationCurrentUser
    {
        public string CurrentUser
        {
            get
            {
                //If you dont use this you will get the following error
                //No scope with a Tag matching 'AutofacWebRequest' is visible from the scope in which the instance was requested
                var salesPortalSessionContext = DependencyResolver.Current.GetService<ISalesPortalSessionContext>();

                if (salesPortalSessionContext.HasValue())
                {
                    return salesPortalSessionContext.SalesPortalSession.UserName;
                }

                return "SalesPortalUser";
            }
        }

        public IEnumerable<Role> CurrentUserRoles
        {
            get
            {
                //If you dont use this you will get the following error
                //No scope with a Tag matching 'AutofacWebRequest' is visible from the scope in which the instance was requested
                var salesPortalSessionContext = DependencyResolver.Current.GetService<ISalesPortalSessionContext>();

                if (salesPortalSessionContext.HasValue())
                {
                    return salesPortalSessionContext.SalesPortalSession.Roles;
                }

                return new List<Role>();
            }
        }

        public string CurrentUserEmailAddress
        {
            get
            {
                //If you dont use this you will get the following error
                //No scope with a Tag matching 'AutofacWebRequest' is visible from the scope in which the instance was requested
                var salesPortalSessionContext = DependencyResolver.Current.GetService<ISalesPortalSessionContext>();

                if (salesPortalSessionContext.HasValue())
                {
                    return salesPortalSessionContext.SalesPortalSession.EmailAddress;
                }

                return "SalesPortalUserEmailAddress";
            }
        }

        public string CurrentUserGivenName
        {
            get
            {
                //If you dont use this you will get the following error
                //No scope with a Tag matching 'AutofacWebRequest' is visible from the scope in which the instance was requested
                var salesPortalSessionContext = DependencyResolver.Current.GetService<ISalesPortalSessionContext>();

                if (salesPortalSessionContext.HasValue())
                {
                    return salesPortalSessionContext.SalesPortalSession.GivenName;
                }

                return "SalesPortalUserGivenName";
            }
        }

        public string CurrentUserSurname
        {
            get
            {
                //If you dont use this you will get the following error
                //No scope with a Tag matching 'AutofacWebRequest' is visible from the scope in which the instance was requested
                var salesPortalSessionContext = DependencyResolver.Current.GetService<ISalesPortalSessionContext>();

                if (salesPortalSessionContext.HasValue())
                {
                    return salesPortalSessionContext.SalesPortalSession.Surname;
                }

                return "SalesPortalUserSurname";
            }
        }
    }
}
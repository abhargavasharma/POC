using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCTestAPP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
              );

            routes.MapRoute(
                name: "Bhanu",
                url: "{controller}/{action}/{id}/{type}/{lathokre}",
                defaults: new { controller = "Bhanu", action = "Telugu", id = UrlParameter.Optional, type = UrlParameter.Optional, lathokre = UrlParameter.Optional }
              );
        }
    }
}

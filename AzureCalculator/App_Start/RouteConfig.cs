using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AzureCalculator
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "Home/{action}/{id}",
                defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TestDrive",
                url: "{isvName}/{testDriveName}",
                defaults: new { controller = "Route", action = "Index", isvName = UrlParameter.Optional, testDriveName = UrlParameter.Optional }
            );
        }
    }
}

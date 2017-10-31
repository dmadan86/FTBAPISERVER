using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FTBAPISERVER
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
              name: "FileManager",
              url: "{controller}/{action}/{id}/{data}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional ,data = UrlParameter.Optional }
          );
            routes.MapRoute(
              name: "FileManagerRoot",
              url: "{controller}/{action}/{id}/{data}/{matter}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, data = UrlParameter.Optional, matter = UrlParameter.Optional }
          );

        }
    }
}

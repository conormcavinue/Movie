using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MovieBrowser.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(null,
                "",
                new {
                    controller = "Movie", action = "List",
                    genre = (string)null, page = 1
                }
            );

            routes.MapRoute(null,
                "Page{page}",
                new { controller = "Movie", action = "List", genre = (string)null },
                new { page = @"\d+" }
            );

            routes.MapRoute(null,
                "{genre}",
                new { controller = "Movie", action = "List", page = 1 }
            );

            routes.MapRoute(null,
                "{genre}/Page{page}",
                new { controller = "Movie", action = "List" },
                new { page = @"\d+" }
            );

            routes.MapRoute(null, "{controller}/{action}");
        }
    }
}
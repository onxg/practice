﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Practice
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Account", action = "Login" }
            );

            routes.MapRoute(
                name: "Charts",
                url: "charts",
                defaults: new { controller = "Side", action = "Charts" }
            );

            routes.MapRoute(
                name: "Register",
                url: "register",
                defaults: new { controller = "Side", action = "Register" }
            );

            routes.MapRoute(
                name: "Forgotten",
                url: "forgotten",
                defaults: new { controller = "Side", action = "Forgot" }
            );
            routes.MapRoute(
                name: "Tables",
                url: "tables",
                defaults: new { controller = "Side", action = "Tables" }
            );
            routes.MapRoute(
                name: "NotFound",
                url: "notfound",
                defaults: new { controller = "Side", action = "NotFound" }
            );
            routes.MapRoute(
                name: "Blank",
                url: "blank",
                defaults: new { controller = "Side", action = "Blank" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

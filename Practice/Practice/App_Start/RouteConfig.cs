namespace Practice
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Employees",
                url: "employees",
                defaults: new { controller = "Employee", action = "Index" }
            );

            routes.MapRoute(
                name: "Stores",
                url: "stores",
                defaults: new { controller = "Store", action = "Index" }
            );

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
               name: "Index",
               url: "index",
               defaults: new { controller = "Home", action = "Index" }
           );

            routes.MapRoute(
                name: "Register",
                url: "register",
                defaults: new { controller = "Account", action = "Register" }
            );

            routes.MapRoute(
                name: "ForgotPassword",
                url: "forgot-password",
                defaults: new { controller = "Account", action = "ForgotPassword" }
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

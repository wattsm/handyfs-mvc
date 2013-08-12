using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HandyFS.Mvc.Example.Web {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "", new { controller = "User", action = "Form" });
            routes.MapRoute("User.Form", "User/Form", new { controller = "User", action = "Form" });
            routes.MapRoute("User.Confirm", "User/{userName}/Confirm", new { controller = "User", action = "Confirm" });
        }
    }
}
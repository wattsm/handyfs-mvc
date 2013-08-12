using System.Web;
using System.Web.Mvc;

namespace HandyFS.Mvc.Example.Web {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
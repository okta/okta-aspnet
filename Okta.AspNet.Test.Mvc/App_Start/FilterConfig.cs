using System.Web;
using System.Web.Mvc;

namespace Okta.AspNet.Test.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

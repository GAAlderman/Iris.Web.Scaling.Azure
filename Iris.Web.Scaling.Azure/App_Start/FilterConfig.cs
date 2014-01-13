using System.Web;
using System.Web.Mvc;

namespace Iris.Web.Scaling.Azure
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
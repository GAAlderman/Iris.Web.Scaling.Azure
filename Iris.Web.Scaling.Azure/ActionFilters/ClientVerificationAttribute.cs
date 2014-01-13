using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Iris.Web.Scaling.Azure.ActionFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientVerificationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var token = actionContext.Request.Headers.GetValues("Authorization-Token").First();
                if (token != null && !string.IsNullOrEmpty(token)) return;
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("Unauthorized client") };
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Missing Authorization-Token")
                };
            }
        }
    }
}
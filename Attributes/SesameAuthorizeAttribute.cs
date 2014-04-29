using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Mvc.Attributes
{
    public class SesameAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            CheckIfUserAuthenticated(filterContext);
        }

        /// <summary>
        ///     Method used to handle HttpUnAuthorizedResult during AjaxRequest.
        /// </summary>
        /// <param name="filterContext"></param>
        private static void CheckIfUserAuthenticated(AuthorizationContext filterContext)
        {
            if(filterContext.Result == null)
            {
                return;
            }

            //Process to clear all the cookies.
            foreach (var cookieKey in filterContext.HttpContext.Request.Cookies.AllKeys)
            {
                var cookie = filterContext.HttpContext.Request.Cookies[cookieKey];
                if (cookie != null)
                    cookie.Expires = DateTime.Now.AddDays(-1);
            }

            //The following block of code helps to detect unauthorized 
            //request and throws 401 error while doing Ajax request.
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }
            filterContext.HttpContext.Response.StatusCode = 401;
            filterContext.HttpContext.Response.AddHeader("LoginUrl", FormsAuthentication.LoginUrl);
            filterContext.HttpContext.Response.End();
        }
    }

}

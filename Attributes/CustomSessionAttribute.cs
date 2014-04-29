using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

 public class CustomSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            if (session == null)
            {
                throw new NullReferenceException("Session not enabled for TOMTRA application");
            }

            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                return;
            }

            /*
             * 1. Check whether session is new and login information available.
             * 2. If requested path is not LoginUrl, then we need to abandon the session and redirect to login page.
             * 3. In case of ajax request, set the response status code as "401", add login url in header and end the response.
             */
            if (session.IsNewSession && (filterContext.HttpContext.User is GenericPrincipal) &&
                filterContext.HttpContext.Request.Path != FormsAuthentication.LoginUrl)
            {
                session.Clear();
                session.Abandon();

                //Process to clear all the cookies.
                foreach (var cookieKey in filterContext.HttpContext.Request.Cookies.AllKeys)
                {
                    var cookie = filterContext.HttpContext.Request.Cookies[cookieKey];
                    if (cookie != null)
                        cookie.Expires = DateTime.Now.AddDays(-1);
                }

                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.HttpContext.Response.AddHeader("LoginUrl", FormsAuthentication.LoginUrl);
                    filterContext.HttpContext.Response.End();
                }
            }
        }
    }

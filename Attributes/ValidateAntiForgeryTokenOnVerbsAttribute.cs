using System;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false,
        Inherited = true)]
    public class ValidateAntiForgeryTokenOnVerbsAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;

        public ValidateAntiForgeryTokenOnVerbsAttribute()
            : this(HttpVerbs.Post)
        {
        }

        public ValidateAntiForgeryTokenOnVerbsAttribute(HttpVerbs verbs)
        {
            _validator = new ValidateAntiForgeryTokenAttribute();
            _verbs = new AcceptVerbsAttribute(verbs);
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //Login page is exempted from validate antiforgerytoken checks
            if (filterContext.RequestContext.RouteData.GetRequiredString("controller") == "Default" &&
               (filterContext.RequestContext.RouteData.GetRequiredString("action") == "Login" 
                    || filterContext.RequestContext.RouteData.GetRequiredString("action") == "IsHyperProfile"))
            { return; }

            var request = filterContext.HttpContext.Request;

            var httpMethodOverride = request.GetHttpMethodOverride();

            if (_verbs.Verbs.Contains(httpMethodOverride))
            {
                _validator.OnAuthorization(filterContext);
            }
        }
    }
}

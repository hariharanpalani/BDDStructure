using System;
using System.Web.Mvc;

public class AjaxAttribute : ActionMethodSelectorAttribute
{
    public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
    {
        if (controllerContext == null)
        {
            throw new NullReferenceException("Controller context should not be null");
        }

        if (methodInfo == null)
        {
            throw new NullReferenceException("Method Info cannot be null");
        }

        var xmlRequest = controllerContext.RequestContext.HttpContext.Request.Headers["X-Requested-With"];

        if (string.IsNullOrEmpty(xmlRequest))
        {
            return false;
        }

        return xmlRequest.ToUpper() == "XMLHTTPREQUEST";
    }
}

using System;
using System.Web;
using System.Web.Mvc;
using Elmah;

public class ElmahErrorAttribute : HandleErrorAttribute
{
    private string _errorId = string.Empty;

    public override void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;

        /*
        Logic to return in case of filter
        */

        LogException(context.Exception);
        ErrorSignal.FromContext(HttpContext.Current).Raise(context.Exception);
        RedirectToErrorPage(context);
    }

    private void RedirectToErrorPage(ExceptionContext context)
    {
        var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
        var errorUrl = urlHelper.Action("Error", "Home", new
        {
            @area = "Global",
            errorid = _errorId
        });

        if (!context.HttpContext.Request.IsAjaxRequest())
        {
            context.HttpContext.Response.Redirect(errorUrl);
        }
        else
        {
            context.HttpContext.Response.AddHeader("ErrorType", "ApplicationError");
            context.HttpContext.Response.AddHeader("ErrorUrl", errorUrl);
            context.HttpContext.Response.End();
        }
    }

    private void LogException(Exception e)
    {
        var context = HttpContext.Current;
        _errorId = ErrorLog.GetDefault(context).Log(new Error(e, context));
    }
}

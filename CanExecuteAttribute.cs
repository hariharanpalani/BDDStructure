using System;
using System.Linq;
using System.Web.Mvc;

public class CanExecuteAttribute : ActionMethodSelectorAttribute
{
    private readonly string _key = "FormSubmit";
    private readonly string[] _values;

    public CanExecuteAttribute(string value)
    {
        _values = value.Split(',');
    }

    public CanExecuteAttribute(string key, string value)
        :this(value)
    {
        _key = key;
    }

    public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
    {
        if (controllerContext == null)
        {
            throw new ArgumentNullException("Controller context is mandatory");
        }

        if (string.IsNullOrEmpty(_key))
        {
            throw new ArgumentNullException("Request key is mandatory");
        }

        var value = controllerContext.RequestContext.HttpContext.Request[_key];

        if (!string.IsNullOrEmpty(value))
        {
            return _values.Contains(value);
        }

        return false;
    }
}

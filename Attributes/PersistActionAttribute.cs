using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

public class PersistActionAttribute : ActionFilterAttribute
    {
        public string Key { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var keyList = Key.Split(',').ToList();

            foreach (var keyStr in keyList)
            {
                filterContext.Controller.TempData.Keep(keyStr);
            }
        }    
    }

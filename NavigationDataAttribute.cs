 using System;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Base;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class NavigationDataAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Override method for handling NavData information one action exection.
        ///     This include the following steps
        ///     1. If the request is of type Ajax, NavData will be taken from Request.Header informtaion
        ///        which has been taken care in OnAjaxSend event.
        ///     2. If it is normal request, if it is postback, then NavData will be taken from Request.Form 
        ///        otherwise, request.QueryString will provide us the NavData.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var request = filterContext.RequestContext.HttpContext.Request;
            var baseController = filterContext.Controller as BaseController;
            string navigationInfo = null;

            if (baseController == null)
                return;

            if (!request.IsAjaxRequest())
            {
                navigationInfo = request.Form["NavData"] ?? request.QueryString["NavData"];
            }
            else
            {
                navigationInfo = request.Headers["NavData"];
            }

            if (string.IsNullOrEmpty(navigationInfo))
            {
                baseController.NavigationData = new NavigationInfo();
                UpdateNavigationData(request, baseController.NavigationData);
                return;
            }

            //Set the tempData with the current NavigationInfo string. This item will be used in the 
            //master layout to sync the NavData hidden field.
            filterContext.Controller.TempData["__NavData"] = navigationInfo;

            //Usually NavigationInfo will be encrypted using Base64 algorithm. So, we need to deserialize the 
            //NavData using Base64 string.
            var encodedByteArray = Convert.FromBase64String(navigationInfo);
            var navDataString = Encoding.UTF8.GetString(encodedByteArray);
            //Deserialized Base64 string will be converted into NavigationData using NewtonSoft API.
            baseController.NavigationData = JsonConvert.DeserializeObject<NavigationInfo>(navDataString);
            //Sync the navigation info from NavigationForm element.
            UpdateNavigationData(request, baseController.NavigationData);
        }

        /// <summary>
        ///     Method used to encrypt the navigation info and attach as querystring
        ///     in all the request.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var redirectResult = filterContext.Result as RedirectToRouteResult;

            //No need to proceed with encryption when action result is not RedirectToRoute or Ajax yype.
            if (redirectResult == null && !filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                return;

            //Convention: All Controller will be derived from BaseController.
            var baseController = filterContext.Controller as BaseController;

            if (baseController == null)
                return;

            var navData = baseController.NavigationData ?? new NavigationInfo();
            //The following block of code will be used to reset some of properties to
            //default values in order to avoid inclusion of these properties during serialization.
            navData.Area = string.Empty;
            navData.Controller = string.Empty;
            navData.Action = string.Empty;

            var navDataString = JsonConvert.SerializeObject(navData, new JsonSerializerSettings
                                                                                               {
                                                                                                   DefaultValueHandling = DefaultValueHandling.Ignore

                                                                                               });
            //Using Base64 algorithm, encrypt the request navData
            var encodedBytes = Encoding.UTF8.GetBytes(navDataString);
            var encodedString = Convert.ToBase64String(encodedBytes);
            //Update the TempData to reflect the same in the HTML.
            filterContext.Controller.TempData["__NavData"] = encodedString;

            //If the request is of type Ajax, then NavData will be set in Response header.
            //These headers will be used by client side ajax event to update the hidden field
            //to sync up the request.
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                filterContext.RequestContext.HttpContext.Response.AddHeader("__NavData", encodedString);

            if (redirectResult != null)
                redirectResult.RouteValues.Add("NavData", encodedString);
        }

        /// <summary>
        ///     Method used to deserialize the NavData information from NavigationInfo path 
        ///     in the form object. This is will help us to restore te modified information
        ///     while performing the submission of NavigationForm through javascript.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="navigationInfo"></param>
        private void UpdateNavigationData(HttpRequestBase request, NavigationInfo navigationInfo)
        {
            var navigationData = request.Form["navigateInfo"];

            if (string.IsNullOrEmpty(navigationData))
                return;

            var jTokenNavigationData = JToken.Parse(navigationData);

            //NewtonSoft will consider every object as JProperty.
            //This will be interated to match with the property in NavigationInfo type.
            //If NavigationInfo class has any matched properties, then we will set the
            //value from JProperty to corresponding type in NavigationInfo using reflection.
            foreach (JProperty jTokenData in jTokenNavigationData)
            {
                var property = navigationInfo.GetType().GetProperty(jTokenData.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                    continue;

                if (jTokenData.Value is JValue)
                {
                    var value = ((JValue)jTokenData.Value).Value;
                    //In case of Nullable properties, we need to consider taking the Underlying property type
                    //to set the data. Otherwise, reflection will throw error during setValue.
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    property.SetValue(navigationInfo, Convert.ChangeType(value, propertyType), null);
                }
                else
                {
                    property.SetValue(navigationInfo, (dynamic)jTokenData.Value, null);
                }
            }
        }
    }

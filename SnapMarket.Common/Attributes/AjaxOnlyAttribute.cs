﻿using System;
using Microsoft.AspNetCore.Mvc.Filters;
using SnapMarket.Common.Extensions;

namespace SnapMarket.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                base.OnActionExecuting(filterContext);
            else
                throw new InvalidOperationException("This operation can only be accessed via Ajax requests");
        }
    }
}

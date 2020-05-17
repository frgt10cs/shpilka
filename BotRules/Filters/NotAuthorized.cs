using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;

namespace BotRules.Filters
{
    public class NotAuthorized : Attribute, IAuthorizationFilter 
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = new RedirectToRouteResult(new RouteValueDictionary()
                {
                    { "controller","home"},
                    { "action" , "index"}
                });
        }
    }
}

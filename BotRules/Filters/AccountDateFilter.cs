using BotRules.Data;
using BotRules.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Filters
{
    public class AccountDateFilter : IAsyncActionFilter
    {
        BotRulesContext context;
        public AccountDateFilter(BotRulesContext context)
        {
            this.context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {            
            User user = await this.context.Users.FirstOrDefaultAsync(u => u.Login == context.HttpContext.User.Identity.Name);
            if (user == null)
            {
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Login" }
                });
            }
            else
                await next();
        }
    }
}

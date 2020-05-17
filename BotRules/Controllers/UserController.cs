using BotRules.Filters;
using BotRules.Models;
using BotRules.Models.Bot;
using BotRules.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotRules.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(AccountDateFilter))]
    public class UserController : Controller
    {
        IBotService botServ;
        IAccountService accServ;
        public UserController(IBotService botServ, IAccountService accServ)
        {
            this.botServ = botServ;
            this.accServ = accServ;
        }

        public async Task<IActionResult> Cabinet()
        {
            User user = await accServ.GetCurrentUserAsync(User.Identity.Name);
            if (user.ConfirmEmail)
                return View(await botServ.GetBotsByMasterIdAsync(user.Id));
            return View();

        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);           
            return RedirectToAction("Index", "Home");
        }
    }
}

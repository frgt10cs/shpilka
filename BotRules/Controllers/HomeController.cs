using System;
using Microsoft.AspNetCore.Mvc;

namespace BotRules.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult AboutUs() => View();
    }
}

using BotRules.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BotRules.Services.Interfaces;
using BotRules.ViewModels;
using BotRules.Filters;
using BotRules.Models.Bot;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;

namespace BotRules.Controllers
{
    [ServiceFilter(typeof(AccountDateFilter))]
    [Authorize]
    public class BotController : Controller
    {
        private IBotService botServ;
        private IAccountService accServ;
        public BotController(IBotService botServ, IAccountService accServ)
        {
            this.botServ = botServ;
            this.accServ = accServ;
        }

        [HttpGet]
        public async Task<IActionResult> Editor(int botId)
        {
            Bot bot = await botServ.GetBotsByIdAsync(botId);
            if (bot != null && bot.MasterId == (await accServ.GetCurrentUserAsync(User.Identity.Name)).Id)
            {
                return View(new BotEditViewModel()
                {
                    Id = bot.Id,
                    Name = bot.Name,
                    Functional = await bot.GetFunctionalAsync()
                });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<JsonResult> GetFunctional(int botId)
        {
            Bot bot = await botServ.GetBotsByIdAsync(botId);
            if (bot != null && bot.MasterId == (await accServ.GetCurrentUserAsync(User.Identity.Name)).Id)
                return Json(await bot.GetFunctionalAsync());
            return Json(false);
        }

        [HttpGet]
        public IActionResult CreateBot() => View();
        
        [HttpPost]
        public async Task<JsonResult> CreateBot([FromBody]CreateBotViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool suc = await botServ.CreateBotAsync(model, await accServ.GetCurrentUserAsync(User.Identity.Name));
                if(suc)
                    return Json(true);                
            }
            return Json("Ошибка регистрации бота. Убедитесь, что имя содержит только латинские символы. Проверьте правильность ключа. Убедитесь, чтоб бот уже не зарегистрирован на нашем сайте.");
        }

        [HttpPost]
        public async Task<JsonResult> SaveFunctional([FromBody]Functional functional)
        {
            Bot bot = await botServ.GetBotsByIdAsync(functional.BotId);            
            if(bot != null && bot.MasterId == (await accServ.GetCurrentUserAsync(User.Identity.Name)).Id)
            {
                // Если бот неактивен, просто сохраняем функционал
                // иначе - сохраняем функционал и подгружаем его
                BotActivity botActive = BotStorage.ActivityBots.FirstOrDefault(b => b.Id == functional.BotId);
                if (botActive != null)
                    await botActive.SaveAndUploadFunctionalAsync(functional);
                else
                    await bot.SaveFunctionalAsync(functional);
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<JsonResult> TurnBot([FromBody]TurnRequest request)
        {
            if (request != null)
            {
                await botServ.TurnBotAsync(request, (await accServ.GetCurrentUserAsync(User.Identity.Name)).Id);
                return Json(true);
            }               
            return Json(false);
        }     
        
        [HttpPost]
        public async Task<IActionResult> RemoveBot(int botId)
        {
            await botServ.RemoveBot(botId, await accServ.GetCurrentUserAsync(User.Identity.Name));
            return RedirectToAction("Cabinet", "User");
        }
    }
}

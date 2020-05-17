using BotRules.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotRules.Controllers
{
    public class TelegramController:Controller
    {
        private IBotService botServ;
        public TelegramController(IBotService botServ)
        {
            this.botServ = botServ;
        }

        /// <summary>
        /// Обращение к телеграм боту
        /// </summary>
        /// <param name="update"></param>
        /// <param name="botName"></param>
        /// <returns></returns>
        [IgnoreAntiforgeryToken]
        [Route(@"telegram/updatetelegrambot/{botName}")]
        public async Task<OkResult> UpdateTelegramBot([FromBody]Update update, [FromRoute]string botName)
        {
            if(update != null && botName != null)
                await botServ.ExecuteFunctionAsync(update, botName);
            return Ok();
        }
    }
}

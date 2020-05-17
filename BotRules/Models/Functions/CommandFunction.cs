using BotRules.Models.Bot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TLSharp.Core;

namespace BotRules.Models.Functions
{
    /// <summary>
    /// Функция, выполняющаяся по команде
    /// </summary>
    public class CommandFunction
    {
        public string Command { get; set; }             
        public BotMessageResponse Response { get;set; }
        public List<InputFunction> InputFunctions { get; set; }

        public CommandFunction()
        {
            InputFunctions = new List<InputFunction>();
        }

        public async virtual void Execute(Message message, BotActivity bot, BotSession session)
        {            
            if (!String.IsNullOrWhiteSpace(Response.Answer))
                await bot.SendTextMessageAsync(new SendMessageArgs() { ChatId = message.Chat.Id, Answer = Response.Answer });
        }
    }
}

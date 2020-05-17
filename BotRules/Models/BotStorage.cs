using BotRules.Models.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public static class BotStorage
    {
        //public static List<TelegramBot> TelegramBots = new List<TelegramBot>();
        public static List<BotActivity> ActivityBots = new List<BotActivity>();
        //public static void AddTelegramBot(TelegramBot bot)
        //{
        //    if (TelegramBots.FirstOrDefault(b => b.Name == bot.Name) == null)
        //        TelegramBots.Add(bot);
        //}

        public static void AddActivityBot(BotActivity bot)
        {
            if (ActivityBots.FirstOrDefault(b => b.Name == bot.Name) == null)
                ActivityBots.Add(bot);
        }
    }
}

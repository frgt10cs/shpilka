using BotRules.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using static System.Net.Mime.MediaTypeNames;

namespace BotRules.Models.Bot
{
    public class TelegramBot : BotActivity
    {
        private TelegramBotClient Client;

        public TelegramBot(string name, string key) : base(name, key)
        {
            // регистрация бота          
            BotStorage.AddActivityBot(this);
        }

        public async override Task<object> GetClient()
        {
            try
            {
                if (Client != null)
                    return Client;
                Client = new TelegramBotClient(Key);
                Url = "https://botrules20181127110642.azurewebsites.net" + "/telegram/updateTelegramBot/" + Name;
                await Client.SetWebhookAsync(Url);
                return Client;
            }
            catch
            {
                return null;
            }
        }

        public Bot TakeAsParent()
        {
            return new Bot()
            {
                DateOfReg = DateOfReg,
                IsActive = IsActive,
                Key = Key,
                LastActivity = LastActivity,
                MasterId = MasterId,
                Name = Name,
                Url = Url
            };
        }

        public async override Task SendTextMessageAsync(SendMessageArgs args)
        {            
            if(!String.IsNullOrWhiteSpace(args.Answer))
                await Client.SendTextMessageAsync(args.ChatId, args.Answer,Telegram.Bot.Types.Enums.ParseMode.Html);
            if(!String.IsNullOrWhiteSpace(args.Photo))
                await Client.SendPhotoAsync(args.ChatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(File.OpenRead(AppConfig.UsersPath + "/" + MasterId + "/files/images/" + args.Photo)));
        }
    }
}

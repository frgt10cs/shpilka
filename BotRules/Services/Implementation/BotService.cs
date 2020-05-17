using BotRules.Data;
using BotRules.Models;
using BotRules.Models.Bot;
using BotRules.Models.Functions;
using BotRules.Services.Interfaces;
using BotRules.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotRules.Services.Implementation
{
    public class BotService : IBotService
    {
        private BotRulesContext context;
        public BotService(BotRulesContext context)
        {
            this.context = context;
        }

        public async Task<BotSession> GetSessionByUserIdAsync(int userId)
        {
            return await context.Sessions.FirstOrDefaultAsync(s => s.UserId == userId);
        }       

        public void AddStorage()
        {

        }

        /// <summary>
        /// Создание бота (только телеграм)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="masterId"></param>
        /// <returns></returns>
        public async Task<bool> CreateBotAsync(CreateBotViewModel model, Models.User user)
        {
            if (user.ConfirmEmail && user.BotCount < 3 && (await context.Bots.FirstOrDefaultAsync(b => b.Key == model.Key || b.Name == model.Name)) == null)
            {
                TelegramBot bot = new TelegramBot(model.Name, model.Key)
                {
                    DateOfReg = DateTime.Now,
                    LastActivity = DateTime.Now,
                    MasterId = user.Id,
                    IsActive = true,                    
                };
                await bot.GetClient();                         
                Bot bot_ = bot.TakeAsParent();
                context.Bots.Add(bot_);
                user.BotCount++;
                await context.SaveChangesAsync();               
                bot.Id = bot_.Id;
                Directory.CreateDirectory(AppConfig.UsersPath + $"/{user.Id}/bots/{bot.Id}/functional");
                bot.PhysicalPath = bot_.PhysicalPath = AppConfig.UsersPath + $"/{bot.MasterId}/bots/{bot_.Id}";                
                await bot_.SaveFunctionalAsync(new Functional()
                {
                    BotId = bot.Id,
                    CommandFunctions = bot.Functions
                });
                await context.SaveChangesAsync();               
                return true;
            }
            return false;
        }

        public async Task<List<Bot>> GetBotsByMasterIdAsync(int masterId)
        {
            return await context.Bots.Where(b => b.MasterId == masterId).ToListAsync();
        }

        /// <summary>
        /// Выполнение функции
        /// </summary>
        /// <param name="update"></param>
        /// <param name="botName"></param>
        /// <returns></returns>
        public async Task ExecuteFunctionAsync(Update update, string botName)
        {
            bool executed = false;
            BotActivity bot = BotStorage.ActivityBots.FirstOrDefault(b => b.Name == botName);
            if (bot != null)
            {
                BotSession session = await GetSessionByUserIdAsync(update.Message.From.Id);   

                // если сессии с этим клиентом нет, создаем новую
                if (session == null)
                {
                    session = new BotSession()
                    {
                        BotId = bot.Id,
                        LastActivity = DateTime.Now,
                        NextFunctionId = 0,
                        UserId = update.Message.From.Id,
                        CurrentCommandFunction = null
                    };
                    context.Sessions.Add(session);
                }

                // функционал бота
                Functional functional = await bot.GetFunctionalAsync();

                // указатель на следующую функцию
                int functionId = session.NextFunctionId;

                // текущая командная функция
                string curBotComFunct = session.CurrentCommandFunction;                

                string message = update.Message.Text.ToLower();

                if (message == "cmds")
                {                    
                    string commands = "";
                    foreach (var func in bot.Functions)
                        commands += func.Command + " " + func.InputFunctions.Count();
                    await bot.SendTextMessageAsync(new SendMessageArgs() { ChatId = update.Message.Chat.Id, Answer = "Доступные команды: " + commands + " " });
                    executed = true;
                }
                if (curBotComFunct == null)
                {
                    // выполнение по команде
                   
                    foreach (CommandFunction func in bot.Functions)
                    {                        
                        string funcCommand = func.Command;
                        if (message.Contains(funcCommand.ToLower()))
                        {
                            func.Execute(update.Message, bot, session);
                            executed = true;
                            // после выполенени командной функции, проверяем следующую на авто
                            var inputFunctions = bot.GetInputFunctions(functional, funcCommand);
                            if (inputFunctions != null)
                            {
                                int inputFunctionsCount = inputFunctions.Count();
                                if (inputFunctionsCount > 0)
                                {
                                    // если функции есть, устанавливам имя на текущую и смотрим, авто ли следующая
                                    session.CurrentCommandFunction = funcCommand;
                                    session.NextFunctionId = 0;
                                    InputFunction nextFunction = bot.GetInputFunction(functional, funcCommand, session.NextFunctionId);                                                                            
                                    // пока идут авто функции, выполняем их
                                    while (nextFunction != null && nextFunction.IsAuto)
                                    {                                        
                                        var funct = nextFunction.GetAsFunction();
                                        await funct.ExecuteAsync(bot, new SendMessageArgs() { ChatId = update.Message.Chat.Id, Answer = "Hello!", Message = message });
                                        session.NextFunctionId = IncId(session.NextFunctionId, inputFunctionsCount);
                                        await context.SaveChangesAsync();
                                        // если при выполнении авто функций мы выполнили все функции, выходим из командной функции
                                        if (session.NextFunctionId == 0)
                                        {
                                            session.CurrentCommandFunction = null;
                                            break;
                                        }
                                        nextFunction = bot.GetInputFunction(functional, curBotComFunct, session.NextFunctionId);
                                    }
                                    // если первая функция не авто, то остается имя текущей командной функции и ссылка на первую функцию
                                }
                            }                                                                  
                            break;
                        }
                    }
                }
                else
                {
                    // количество фукнций в цепочке
                    var cmndf = (await bot.GetFunctionalAsync()).CommandFunctions.FirstOrDefault(f => f.Command == curBotComFunct);
                    if (cmndf != null)
                    {
                        var fus = cmndf.InputFunctions;
                        if (fus != null)
                        {
                            int inputFunctionsCount =fus.Count();
                            if (functionId < inputFunctionsCount)
                            {
                                var function = bot.Functions.FirstOrDefault(f => f.Command == curBotComFunct).InputFunctions[functionId];
                                //InputFunction function = bot.GetInputFunction(functional, curBotComFunct, functionId);
                                if (function != null)
                                {
                                    await function.ExecuteAsync(bot, new SendMessageArgs() { ChatId = update.Message.Chat.Id, AuthorId = update.Message.From.Id, Message = message });
                                    executed = true;
                                    // установление ссылки на след функцию
                                    session.NextFunctionId = IncId(functionId, inputFunctionsCount);
                                    if (session.NextFunctionId == 0)
                                        session.CurrentCommandFunction = null;
                                    else
                                    {
                                        //// если следующая функция авто, вызываем сразу
                                        InputFunction nextFunction = bot.GetInputFunction(functional, curBotComFunct, session.NextFunctionId);
                                        while (nextFunction != null && nextFunction.IsAuto)
                                        {
                                            var funct = nextFunction.GetAsFunction();
                                            await funct.ExecuteAsync(bot, new SendMessageArgs() { ChatId = update.Message.Chat.Id, Answer = "Hello!", Message = message });
                                            session.NextFunctionId = IncId(session.NextFunctionId, inputFunctionsCount);
                                            // если при выполнении авто функций мы выполнили все функции, выходим из командной функции
                                            if (session.NextFunctionId == 0)
                                            {
                                                session.CurrentCommandFunction = null;
                                                break;
                                            }
                                            nextFunction = bot.GetInputFunction(functional, curBotComFunct, session.NextFunctionId);
                                        }
                                    }
                                }
                            }
                        }
                    }                                        
                }                
                await context.SaveChangesAsync();
                if (!executed)
                {
                    string answer = "Упс, эту команду я не понимаю :)";
                    if (message.Contains("/start"))
                        answer = "Здравствуйте! Рад встрече :)";
                    else
                        await bot.SendTextMessageAsync(new SendMessageArgs() { ChatId= update.Message.Chat.Id, Answer= answer });
                }
                   
            }
        }

        /// <summary>
        /// Увеличение индекса (ссылки на след функцию). Вовращает ноль, если индекс вышел за пределы массива или списка
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private int IncId(int id, int count)
        {
            id += 1;
            if (id >= count)
                return 0;
            return id;
        }

        /// <summary>
        /// Вовзращает наследника с заполненными полями родителя (пока только телеграм)
        /// </summary>
        /// <param name="bot"></param>
        /// <returns></returns>
        private async Task<TelegramBot> TakeAsBotActivity(Bot bot)
        {
            TelegramBot b = new TelegramBot(bot.Name, bot.Key)
            {
                Id = bot.Id,
                DateOfReg = bot.DateOfReg,
                IsActive = bot.IsActive,
                Key = bot.Key,
                LastActivity = bot.LastActivity,
                MasterId = bot.MasterId,
                Name = bot.Name,
                Url = bot.Url
            };
            await b.GetClient();
            return b;
        }

        /// <summary>
        /// Включение/выключение бота (только телеграм)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="masterId"></param>
        /// <returns></returns>
        public async Task TurnBotAsync(TurnRequest request, int masterId)
        {
            Bot dataBot = await context.Bots.FirstOrDefaultAsync(b => b.Id == request.BotId);            
            BotActivity bot = BotStorage.ActivityBots.FirstOrDefault(b => b.Id == request.BotId);
            if (dataBot != null && dataBot.MasterId == masterId) 
            {
                if (!request.TurnOn)
                {
                    if (bot != null)
                    {                        
                        BotStorage.ActivityBots.Remove(bot);
                        dataBot.IsActive = false;
                    }
                }
                else
                {
                    if (bot == null)
                    {
                        // бот автоматом добавляется в конструкторе
                        dataBot.IsActive = true;
                        BotActivity botActive = await TakeAsBotActivity(dataBot);
                        botActive.Sessions = await context.Sessions.Where(s => s.BotId == botActive.Id).ToListAsync();
                        await botActive.UploadFunctionalAsync();
                    }
                }
                await context.SaveChangesAsync();
            }            
        }

        public async Task<Bot> GetBotsByIdAsync(int botId)
        {
            return await context.Bots.FirstOrDefaultAsync(b => b.Id == botId);
        }

        public async Task RemoveBot(int botId, Models.User user)
        {
            Bot bot = await context.Bots.FirstOrDefaultAsync(b => b.Id == botId);
            if (bot != null && bot.MasterId == user.Id)
            {
                BotActivity ba = BotStorage.ActivityBots.FirstOrDefault(b => b.Id == botId);
                if (ba != null)
                    BotStorage.ActivityBots.Remove(ba);
                context.Bots.Remove(bot);
                user.BotCount=user.BotCount>0?(--user.BotCount):0;
                await context.SaveChangesAsync();
            }
        }
    }
}

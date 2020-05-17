using BotRules.Models.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BotRules.Models.Bot
{
    public abstract class BotActivity:Bot
    {
        public List<CommandFunction> Functions;
        public List<BotSession> Sessions;

        public BotActivity(string name, string key)
        {
            Name = name;
            Key = key;
            Functions = new List<CommandFunction>();
        }

        public void AddFunction(CommandFunction function)
        {
            // приводим каждую функцию к нужному классу
            for (int i = 0; i < function.InputFunctions.Count(); i++)
            {
                function.InputFunctions[i] = function.InputFunctions[i].GetAsFunction();
            }
            Functions.Add(function);
        }        

        /// <summary>
        /// Подгружает функции бота из файла и устанавливает
        /// </summary>
        /// <returns></returns>
        public async Task UploadFunctionalAsync()
        {
            Functions = new List<CommandFunction>();
            Functional functional = await GetFunctionalAsync();
            foreach (var func in functional.CommandFunctions)
                AddFunction(func);
        }

        /// <summary>
        /// Сохраняет функционал бота и вносит изменения
        /// </summary>
        /// <param name="functional"></param>
        /// <returns></returns>
        public async Task SaveAndUploadFunctionalAsync(Functional functional)
        {
            await SaveFunctionalAsync(functional);
            Functions = new List<CommandFunction>();
            foreach (var func in functional.CommandFunctions)
                AddFunction(func);
        }

        public abstract Task SendTextMessageAsync(SendMessageArgs args);

        public abstract Task<object> GetClient();
    }
}

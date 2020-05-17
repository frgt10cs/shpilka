using BotRules.Models.Functions;
using BotRules.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models.Bot
{
    public class Bot : IBot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public int MasterId { get; set; }
        public DateTime DateOfReg { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsActive { get; set; }
        public string PhysicalPath { get; set; }


        /// <summary>
        /// Сохраняет функционал бота в файл
        /// </summary>
        /// <param name="functional"></param>
        /// <returns></returns>
        public async Task SaveFunctionalAsync(Functional functional)
        {
            foreach(var commandFunc in functional.CommandFunctions)
            {
                if (String.IsNullOrWhiteSpace(commandFunc.Command) || String.IsNullOrWhiteSpace(commandFunc.Response.Answer))
                    functional.CommandFunctions.Remove(commandFunc);
                else
                {
                    foreach(var func in commandFunc.InputFunctions)
                    {
                        foreach (string str in func.Arguments)
                            if (String.IsNullOrWhiteSpace(str))
                                commandFunc.InputFunctions.Remove(func);
                    }                        
                }
            }
            string path = AppConfig.UsersPath + $"/{MasterId}/bots/{Id}/functional/";
            await File.WriteAllTextAsync(path + "CommandFunctions.json", JsonConvert.SerializeObject(functional.CommandFunctions));
        }

        /// <summary>
        /// Возвращает функционал бота, записанный в файл
        /// </summary>
        /// <returns></returns>
        public async Task<Functional> GetFunctionalAsync()
        {
            return new Functional()
            {
                CommandFunctions = JsonConvert.DeserializeObject<List<CommandFunction>>(await File.ReadAllTextAsync(AppConfig.UsersPath + $"/{MasterId}/bots/{Id}/functional/CommandFunctions.json"))
            };
        }

        public InputFunction GetInputFunction(Functional functional,string commandName, int functionId)
        {
            return functional.CommandFunctions.FirstOrDefault(f => f.Command == commandName).InputFunctions[functionId];
        }

        public List<InputFunction> GetInputFunctions(Functional functional, string commandName)
        {
            var cf = functional.CommandFunctions.FirstOrDefault(f => f.Command == commandName);
            if(cf != null)
                return cf.InputFunctions;      
            else
                return null;
        }
    }   
}
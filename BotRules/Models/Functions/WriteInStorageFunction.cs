using BotRules.Models.Bot;
using BotRules.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models.Functions
{
    public class WriteInStorageFunction : InputFunction
    {
        public WriteInStorageFunction(string[] args)
        {
            IsAuto = false;
            ArgumentsName = new string[]
            {
                "Ответ",
                "Хранилище"
            };
            Arguments = args;
            FunctionType = FunctionTypes.WriteInStorage;
        }

        public async override Task ExecuteAsync(BotActivity bot, SendMessageArgs args)
        {
            args.Answer = Arguments[0];
            await WriteInStorage(bot.MasterId,  args.AuthorId, Arguments[1].ToString(), args.Message);
            await bot.SendTextMessageAsync(args);
        }

        private async Task WriteInStorage(int ClientId, long userId, string storageName, string Value)
        {
            if (String.IsNullOrWhiteSpace(Value))
                return;
            string Name = AppConfig.UsersPath + $"/{ClientId}/Storages/{storageName}.txt";
            if (File.Exists(Name))
            {
                var readedLines = File.ReadAllLines(Name).ToList();
                string[] fcur = readedLines[0].Split(';');
                for (int i = 0; i < readedLines.Count(); i++)
                {
                    string[] cur = readedLines[i].Split(';');
                    if (cur[0] == userId.ToString() & cur.Length != fcur.Length)
                    {
                        Value = readedLines[i] + ";" + Value;
                        readedLines[i] = Value;
                        File.WriteAllLines(Name, readedLines.ToArray());
                        return;
                    }
                }
                readedLines.Add($"{userId};{Value}");
                File.WriteAllLines(Name, readedLines.ToArray());
            }
        }
    }
}

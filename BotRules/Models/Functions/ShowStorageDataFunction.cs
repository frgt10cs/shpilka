using BotRules.Models.Bot;
using BotRules.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models.Functions
{
    public class ShowStorageDataFunction:InputFunction
    {
        public ShowStorageDataFunction(string[] args)
        {
            IsAuto = true;
            ArgumentsName = new string[]
            {                
                "Хранилище"
            };
            Arguments = args;
            FunctionType = FunctionTypes.ShowDataSorage;
        }

        public async override Task ExecuteAsync(BotActivity bot, SendMessageArgs args)
        {
            args.Answer = "типа данные";
            await bot.SendTextMessageAsync(new SendMessageArgs() { Answer = await GetDataFromStorage(bot.MasterId, Arguments[0].ToString()), ChatId = args.ChatId });            
        }

        public async Task<string> GetDataFromStorage(int userId, string storageName)
        {
            string Name = AppConfig.UsersPath + $"/{userId}/Storages/{storageName}.txt";
            if (!File.Exists(Name))
                return "";
            var readedLines = File.ReadAllLines(Name).ToList();
            int Columns = readedLines[0].Split(';').Length - 1;
            string Data = "";
            string[] local = new string[Columns];
            int[] Max = new int[Columns];
            for (int i = 0; i < readedLines.Count; i++)
            {
                for (int j = 1; j <= Columns; j++)
                {
                    int LocalLength = readedLines[i].Split(';')[j].Length;
                    if (LocalLength > Max[j - 1])
                        Max[j - 1] = LocalLength;
                }
            }
            for (int i = 0; i < Max.Length; i++)
                Max[i] += 2;
            for (int i = 0; i < readedLines.Count; i++)
            {
                if (i != readedLines.Count)
                    Data += "|";
                local = readedLines[i].Split(';').Skip(1).ToArray();
                for (int j = 0; j < Columns; j++)
                {
                    int c = Max[j] - local[j].Length;
                    if (c % 2 == 0)
                        local[j] = new string('−', c / 2) + local[j] + new string('−', c / 2) + "|";
                    else
                        local[j] = new string('−', Convert.ToInt32(c / 2)) + local[j] + new string('−', Convert.ToInt32(c / 2) + 1) + "|";
                }
                foreach (string element in local)
                    Data += element;
                Data += "\n";
            }
            return Data;
        }
    }
}


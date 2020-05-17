using BotRules.Models.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models.Functions
{
    public enum FunctionTypes
    {
        None,
        WriteInStorage,
        ShowDataSorage
    }

    /// <summary>
    /// Функция, выполняющаяся по цепочке
    /// </summary>
    public class InputFunction
    {
        /// <summary>
        /// Какие аргументы нужны для создания функции (их названия)
        /// </summary>
        public string[] ArgumentsName;

        /// <summary>
        /// Значения аргументов
        /// </summary>
        public string[] Arguments;

        /// <summary>
        /// Вызывается ли функция сразу, без ожидания ввода пользователя
        /// </summary>
        public bool IsAuto;

        public FunctionTypes FunctionType;

        /// <summary>
        /// Выполнение метода с заданными аргументами
        /// </summary>
        /// <param name="methodArgs"></param>
        public async virtual Task ExecuteAsync(BotActivity bot,SendMessageArgs args)
        {
            //Type typeOfFS = typeof(FunctionsStorage);
            //var method = typeOfFS.GetMethod(MethodName);
            //if (method != null)
            //    method.Invoke(null, methodArgs);
        }

        public InputFunction GetAsFunction()
        {
            InputFunction ret = null;
            switch (FunctionType)
            {
                case FunctionTypes.WriteInStorage:
                    ret = new WriteInStorageFunction(Arguments);
                    break;
                case FunctionTypes.ShowDataSorage:
                    ret = new ShowStorageDataFunction(Arguments);
                    break;
            }
            return ret;
        }
    }
}
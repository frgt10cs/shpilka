using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models.Bot
{
    /// <summary>
    /// Сессия с данными о пользователе, с которым общается бот
    /// </summary>
    public class BotSession
    {        
        public int Id { get; set; }
        /// <summary>
        /// Айди бота, которому принадлежит сессия
        /// </summary>
        public int BotId { get; set; }
        /// <summary>
        /// Айди с чатом
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// Индекс следующей в цепочке функции
        /// </summary>
        public int NextFunctionId { get; set; }
        /// <summary>
        /// Последняя активность сессии
        /// </summary>
        public DateTime LastActivity { get; set; }
        /// <summary>
        /// Название функции, в которой выполняется цепочка
        /// </summary>
        public string CurrentCommandFunction { get; set; }

        public BotSession()
        {
            //Изначально указывает на первую функцию в цепочку
            NextFunctionId = 0;
        }
    }
}

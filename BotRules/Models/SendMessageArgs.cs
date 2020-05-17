using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public class SendMessageArgs
    {
        public long ChatId { get; set; }
        public string Answer { get; set; }
        public string Photo { get; set; }
        public string Message { get; set; }
        public long AuthorId { get; set; }
    }
}

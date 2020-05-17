using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public class ServerResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public string UrlRedirect { get; set; }
        public string NameRedirect { get; set; }
    }
}

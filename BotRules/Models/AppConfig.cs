using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Services
{
    public static class AppConfig
    {        
        public static string Ip;
        public static string Port;
        public static string AppUrl;
        public static string UsersPath = Environment.CurrentDirectory + "/wwwroot/users";
    }
}

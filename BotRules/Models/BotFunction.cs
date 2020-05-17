using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public class BotFunction
    {
        public int BotId;
        public string Command;
        public int NextFuncId;
        public BotMessageResponse Response;
    }
}

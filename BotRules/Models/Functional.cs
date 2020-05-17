using BotRules.Models.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public class Functional
    {
        public int BotId { get; set; }
        public List<CommandFunction> CommandFunctions { get; set; }
    }
}

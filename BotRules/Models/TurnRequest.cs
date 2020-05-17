using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public class TurnRequest
    {
        public int BotId { get; set; }
        public bool TurnOn { get; set; }
    }
}

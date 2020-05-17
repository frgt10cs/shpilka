using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models
{
    public class ServiceResult
    {
        public bool Successful { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}

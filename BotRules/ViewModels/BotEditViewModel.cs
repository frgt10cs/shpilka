using BotRules.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.ViewModels
{
    public class BotEditViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Functional Functional { get; set; }
    }
}

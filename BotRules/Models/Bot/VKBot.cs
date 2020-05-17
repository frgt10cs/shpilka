using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.Models.Bot
{
    public class VKBot : BotActivity
    {
        public VKBot(string name, string key):base(name, key)
        {

        }

        public override Task<object> GetClient()
        {
            throw new NotImplementedException();
        }

        public override Task SendTextMessageAsync(SendMessageArgs args)
        {
            throw new NotImplementedException();
        }
    }
}

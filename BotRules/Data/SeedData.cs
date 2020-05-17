using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace BotRules.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BotRulesContext(serviceProvider.GetRequiredService<DbContextOptions<BotRulesContext>>()))
            {
                if (context.Bots.Any())
                {
                    foreach (Models.Bot.Bot bot in context.Bots)
                        bot.IsActive = false;
                    context.SaveChanges();
                }                            
            }
        }
    }
}

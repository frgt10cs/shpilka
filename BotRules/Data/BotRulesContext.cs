using BotRules.Models;
using BotRules.Models.Bot;
using Microsoft.EntityFrameworkCore;

namespace BotRules.Data
{
    /// <summary>
    /// Контекст для базы данных приложения
    /// </summary>
    public class BotRulesContext:DbContext
    {
        public BotRulesContext(DbContextOptions<BotRulesContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<BotSession> Sessions { get; set; }
        public DbSet<Bot> Bots { get; set; }
    }
}

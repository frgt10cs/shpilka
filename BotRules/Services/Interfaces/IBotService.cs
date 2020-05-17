using BotRules.Models;
using BotRules.Models.Bot;
using BotRules.Models.Functions;
using BotRules.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotRules.Services.Interfaces
{
    public interface IBotService
    {
        Task<bool> CreateBotAsync(CreateBotViewModel model, BotRules.Models.User user);
        Task<BotSession> GetSessionByUserIdAsync(int userId);
        Task ExecuteFunctionAsync(Update update, string botName);
        Task<List<Bot>> GetBotsByMasterIdAsync(int masterId);
        Task TurnBotAsync (TurnRequest request, int masterId);
        Task<Bot> GetBotsByIdAsync(int botId);
        Task RemoveBot(int botId, BotRules.Models.User user);
    }
}

using BotRules.Models;
using System.Threading.Tasks;
using BotRules.ViewModels;

namespace BotRules.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult> RegistrationAsync(RegistrationViewModel model);
        /// <summary>
        /// Returns null if login is failed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<AuthenticateUser> LoginAsync(LoginViewModel model);
        Task<ServiceResult> ConfirmEmailAsync(string token);
        Task<User> GetCurrentUserAsync(string login);
        string GetUserClaimValue(string type);
        Task<ServiceResult> ResetPassword(string email);
        Task<bool> UpdatePasswordIsSuccessful(string token);
        Task<bool> EmailIsConfirmedAsync(string login);
    }
}

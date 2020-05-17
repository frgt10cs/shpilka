using BotRules.Data;
using BotRules.Models;
using BotRules.Services;
using BotRules.Services.Interfaces;
using BotRules.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.IO;

namespace WebAppCore.Services
{
    /// <summary>
    /// Сервис, отвечающий за работу с аккаунтом
    /// </summary>
    public class AccountService : IAccountService
    {
        private BotRulesContext context;
        private readonly ClaimsPrincipal principal;        
        public AccountService(BotRulesContext context, IPrincipal principal)
        {
            this.context = context;
            this.principal = principal as ClaimsPrincipal;
        }

        public async Task<ServiceResult> RegistrationAsync(RegistrationViewModel model)
        {            
            ServiceResult result = new ServiceResult()
            {
                ErrorMessages = new List<string>(),
                Successful = false
            };
            if (await EmailIsUniq(model.Email))
            {
                if (await LoginIsUniq(model.Login))
                {
                    User user = new User()
                    {
                        ConfirmEmail = false,
                        Email = model.Email,
                        Login = model.Login,
                        Firstname = model.FirstName,
                        LastChanges = DateTime.Now,
                        Lastname = model.LastName,
                        Salt = PasswordManager.GenerateSalt(),
                        Role = "User"
                    };
                    user.PasswordHash = PasswordManager.GenerateHash(model.Password, PasswordManager.MainSalt + user.Salt);
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                    Token token = new Token()
                    {
                        GenerationDate = DateTime.Now,
                        UserId = user.Id,
                        Value = PasswordManager.GenerateToken(15),
                        Action = "coem"
                    };
                    context.Tokens.Add(token);
                    await context.SaveChangesAsync();
                    /*await EmailManager.SendMessageAsync(user.Email,
                        $"Благодарим за регистрацию." +
                        $" Для подтверждения эл. почты перейдите по" +
                        $" <a href=\"https://{AppConfig.AppUrl}/Account/ConfirmEmail?token={token.Value}\"> ссылке </a>",
                        "Shpilka регистрация");*/
                    string directoryName = AppConfig.UsersPath + "/" + user.Id.ToString();
                    Directory.CreateDirectory(directoryName);
                    result.Successful = true;
                }
                else
                    result.ErrorMessages.Add("Пользователь с таким логином уже зарегистрирован");
            }
            else
                result.ErrorMessages.Add("Пользователь с такой электронной почтой уже зарегистрирован");
            return result;
        }

        public async Task<AuthenticateUser> LoginAsync(LoginViewModel model)
        {
            AuthenticateUser auth = null;
            User user = await context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
            if (user != null && PasswordManager.GenerateHash(model.Password, user.Salt) == user.PasswordHash)
            {
                auth = new AuthenticateUser()
                {
                    Id = user.Id,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Login = user.Login
                };
            }
            return auth;
        }       

        public async Task<ServiceResult> ConfirmEmailAsync(string token)
        {
            ServiceResult result = new ServiceResult()
            {
                Successful = false                
            };
            Token _token = await context.Tokens.FirstOrDefaultAsync(t => t.Value == token);
            if (_token != null)
            {
                User user = await context.Users.FirstOrDefaultAsync(u => u.Id == _token.UserId);
                if (user != null && !user.ConfirmEmail)
                {
                    user.ConfirmEmail = true;
                    context.Tokens.Remove(_token);
                    await context.SaveChangesAsync();
                    result.Successful = true;                                                           
                }                    
            }
            return result;
        }

        public async Task<bool> EmailIsConfirmedAsync(string login)
        {
            return (await context.Users.FirstOrDefaultAsync(u => u.Login == login)).ConfirmEmail;
        }

        public async Task<bool> EmailIsUniq(string email)
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user == null;
        }

        public async Task<bool> LoginIsUniq(string login)
        {
            User user = await context.Users.FirstOrDefaultAsync(u => u.Login == login);
            return user == null;
        }

        public async Task<User> GetCurrentUserAsync(string login)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public string GetUserClaimValue(string type)
        {
            return principal.FindFirstValue(type);
        }

        public async Task<ServiceResult> ResetPassword(string email)
        {
            ServiceResult result = new ServiceResult()
            {
                Successful = false,
                ErrorMessages = new List<string>()
            };
            User user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                if (await context.Tokens.FirstOrDefaultAsync(t => t.UserId == user.Id && t.Action == "repa") == null)
                {
                    Token token = new Token()
                    {
                        Action = "repa",
                        GenerationDate = DateTime.Now,
                        UserId = user.Id,
                        Value = PasswordManager.GenerateToken()
                    };
                    context.Tokens.Add(token);
                    await context.SaveChangesAsync();
                    await EmailManager.SendMessageAsync(user.Email,
                            "Для восстановления пароля перейдите по <a href=\"https://" +
                            AppConfig.AppUrl +
                            $"/Account/ResetPassword/{token.Value}\"> ссылке </a>",
                            "Восстановление пароля Shpilka.ru");
                    result.Successful = true;
                    return result;
                }
                else
                    result.ErrorMessages.Add("Письмо со сбросом пароля уже отправлено на эту почту.");
            }
            else
                result.ErrorMessages.Add("Неправильная электронная почта.");
            return result;
        }

        public async Task<bool> UpdatePasswordIsSuccessful(string token)
        {
            Token _token = await context.Tokens.FirstOrDefaultAsync(t => t.Value == token);
            if (_token != null && _token.Action == "repa") 
            {
                User user = await context.Users.FirstOrDefaultAsync(u => u.Id == _token.UserId);
                if (user != null)
                {
                    string password = PasswordManager.GenerateToken();
                    string salt = PasswordManager.GenerateSalt();
                    string hash = PasswordManager.GenerateHash(password, salt);
                    user.PasswordHash = hash;
                    user.Salt = salt;
                    user.LastChanges = DateTime.Now;
                    await EmailManager.SendMessageAsync(user.Email, $"Восстановление пароля проведено успешно!<br>" +
                             $"Ваш новый пароль: {password}<br>" +
                             "Вы можете сменить его на любой другой в любое время в вашем" +
                             " <a href=\"" + AppConfig.AppUrl + "/Account/Cabinet\"> личном кабинете </a>.");
                    context.Tokens.Remove(_token);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
    }
}

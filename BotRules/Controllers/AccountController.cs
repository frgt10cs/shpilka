using BotRules.Attributes.Validation;
using BotRules.Filters;
using BotRules.Models;
using BotRules.Services;
using BotRules.Services.Interfaces;
using BotRules.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BotRules.Controllers
{
    public class AccountController : Controller
    {
        private IAccountService accServ;
        public AccountController(IAccountService accServ)
        {
            this.accServ = accServ;
        }

        [NotAuthorized]
        [HttpGet]
        public IActionResult Registration() => View();

        [NotAuthorized]
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                ServiceResult result = await accServ.RegistrationAsync(model);
                if (result.Successful)
                    return View("Response", new ServerResponse()
                    {
                        Message = "Регистрация завершена успешно! Подтвердите электронную почту, чтобы начать пользоваться аккаунтом.",
                        Successful = true,
                        NameRedirect = "На главную",
                        UrlRedirect = "/"
                    });
                foreach (string error in result.ErrorMessages)
                    ModelState.AddModelError(string.Empty, error);
            }
            else
                ModelState.AddModelError(string.Empty, "Неверно заполнены данные");
            return View(model);
        }

        [NotAuthorized]
        [HttpGet]
        public IActionResult Login() => View();

        [NotAuthorized]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                AuthenticateUser user = await accServ.LoginAsync(model);
                if (user != null)
                {
                    await AuthenticateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (new TokenRequiredSymbols().IsValid(token) && new RequiredAttribute().IsValid(token)
                && new MaxLengthAttribute(20).IsValid(token) && new MinLengthAttribute(10).IsValid(token))
            {
                ServiceResult result = await accServ.ConfirmEmailAsync(token);
                if (result.Successful)
                    return View("Response", new ServerResponse()
                    {
                        Message = "Электронная почта успешно подтверждена!",
                        Successful = true,
                        NameRedirect = "перейти в Личный кабинет",
                        UrlRedirect = "/User/Cabinet"
                    });
            }
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        public async Task AuthenticateAsync(AuthenticateUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim("Firstname", user.Firstname),
                new Claim("Lastname", user.Lastname),
                new Claim("Id", user.Id.ToString())
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        [NotAuthorized]
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [NotAuthorized]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            string errorMessage = "Некорректная эл. почта";
            if (!String.IsNullOrWhiteSpace(email) && email.Length < 100 && email.Length > 6 && new DataTypeAttribute(DataType.EmailAddress).IsValid(email))
            {
                ServiceResult result = await accServ.ResetPassword(email);
                if (result.Successful)
                    return View("Response", new ServerResponse()
                    {
                        Message = "Письмо с ссылкой на восстановление пароля отправлено на вашу эл. почту",
                        NameRedirect = "На главную",
                        Successful = true,
                        UrlRedirect = "/"
                    });
                errorMessage = result.ErrorMessages.First();
            }
            return View("Response", new ServerResponse()
            {
                Message = errorMessage,
                NameRedirect = "Вернуться",
                Successful = false,
                UrlRedirect = "/Account/ForgotPassword"
            });
        }

        [NotAuthorized]
        [HttpGet]
        public async Task<IActionResult> UpdatePassword(string token)
        {
            if (!String.IsNullOrWhiteSpace(token) && token.Length < 20 && token.Length > 10)
            {
                bool result = await accServ.UpdatePasswordIsSuccessful(token);
                if (result)
                    return View("Response", new ServerResponse()
                    {
                        Message = "Письмо с вашим новым паролем было отправлено вам на почту",
                        NameRedirect = "В личный кабинет",
                        Successful = true,
                        UrlRedirect = "/Account/Cabinet"
                    });
            }
            return NotFound();
        }
    }
}

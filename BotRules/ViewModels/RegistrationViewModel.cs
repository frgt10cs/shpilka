using System.ComponentModel.DataAnnotations;
using BotRules.Attributes.Validation;

namespace BotRules.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Заполните поле")]
        [MaxLength(50, ErrorMessage = "Слишком длинное имя. Пожалуйста, напишите неполное имя")]
        [MinLength(2, ErrorMessage = "Имя не может состоять из одной буквы")]
        [NameRequiredSymbols]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [MaxLength(50, ErrorMessage = "Слишком длинное фамилия")]
        [MinLength(2, ErrorMessage = "Фамилия не может состоять из одной буквы")]
        [NameRequiredSymbols]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [MaxLength(20, ErrorMessage = "Логин слишком длинный")]
        [MinLength(4, ErrorMessage = "Логин слишком короткий")]
        [LoginRequiredSymbols]
        public string Login { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [MaxLength(100, ErrorMessage = "Эл. почта имеет слишком длинный адрес")]
        [MinLength(6, ErrorMessage = "Слишком короткая эл. почта")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не похоже, что это адрес эл. почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [MaxLength(50, ErrorMessage = "Пароль слишком длинный")]
        [MinLength(5, ErrorMessage = "Пароль слишком короткий")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [MaxLength(50, ErrorMessage = "Пароль слишком длинный")]
        [MinLength(5 , ErrorMessage = "Пароль слишком короткий")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}

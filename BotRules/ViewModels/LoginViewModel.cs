using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BotRules.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [MaxLength(20, ErrorMessage = "Логин слишком длинный")]
        [MinLength(4, ErrorMessage = "Логин слишком короткий")]
        //[LoginRequiredSymbols]        
        public string Login { get; set; }
        [Required(ErrorMessage = "Введите праоль")]
        [MaxLength(50, ErrorMessage = "Неверный пароль")]
        [MinLength(5, ErrorMessage = "Неверный пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

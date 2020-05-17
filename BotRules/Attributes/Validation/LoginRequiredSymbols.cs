using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BotRules.Attributes.Validation
{
    public class LoginRequiredSymbols: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var regexItem = new Regex("^[a-zA-Z0-9]+$");            
            string str = (string)value;
            if (!String.IsNullOrWhiteSpace(str))
                if (regexItem.IsMatch(str) && (str.IndexOf(' ') == -1)) 
                    return ValidationResult.Success;
            return new ValidationResult("Логин может содержать только латинские символы и цифры");
        }    
    }
}

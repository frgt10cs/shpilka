using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BotRules.Attributes.Validation
{
    public class NameRequiredSymbols : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var regexItem = new Regex("^[a-zA-Zа-яА-Я]+$");
            var regexNum = new Regex("[0-9]");
            string str = (string)value;
            if (!String.IsNullOrWhiteSpace(str))
                if (regexItem.IsMatch(str) && !regexNum.IsMatch(str) && (str.IndexOf(' ') == -1)) 
                    return ValidationResult.Success;
            return new ValidationResult("Имя может содержать только латинские символы и кириллицу");
        }
    }
}
